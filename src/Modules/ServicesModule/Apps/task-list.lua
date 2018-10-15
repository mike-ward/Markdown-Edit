local List = require 'pandoc.List'

local M = {}

local function is_html (format)
  return format == 'html' or format == 'html4' or format == 'html5'
end

--- Create a ballot box for the given output format.
function M.ballot_box (format)
  if is_html(format) then
    return pandoc.RawInline(
      'html',
      '<input type="checkbox" class="task-list-item-checkbox" disabled />'
    )
  elseif format == 'gfm' then
    -- GFM includes raw HTML
    return pandoc.RawInline('html', '[ ]')
  elseif format == 'org' then
    return pandoc.RawInline('org', '[ ]')
  elseif format == 'latex' then
    return pandoc.RawInline('tex', '$\\square$')
  else
    return pandoc.Str '?'
  end
end

--- Create a checked ballot box for the given output format.
function M.ballot_box_with_check (format)
  if is_html(format) then
    return pandoc.RawInline(
      'html',
      '<input type="checkbox" class="task-list-item-checkbox" checked disabled />'
    )
  elseif format == 'gfm' then
    -- GFM includes raw HTML
    return pandoc.RawInline('html', '[x]')
  elseif format == 'org' then
    return pandoc.RawInline('org', '[X]')
  elseif format == 'latex' then
    return pandoc.RawInline('tex', '$\\rlap{$\\checkmark$}\\square$')
  else
    return pandoc.Str '?'
  end
end

--- Replace a Github-style task indicator with a bullet box representation
--- suitable for the given output format.
function M.todo_marker (inlines, format)
  if (inlines[1] and inlines[1].text == '[' and
      inlines[2] and inlines[2].t == 'Space' and
      inlines[3] and inlines[3].text == ']') then
    return M.ballot_box(format), 3
  elseif (inlines[1] and
            (inlines[1].text == '[x]' or
             inlines[1].text == '[X]')) then
    return M.ballot_box_with_check(format), 1
  else
    return nil, 0
  end
end

M.css_styles = [[
<style>
  .task-list-item {
    list-style-type: none;
  }
  .task-list-item-checkbox {
    margin-left: -1.6em;
  }
</style>
]]

--- Add task-list CSS styles to the header.
function M.add_task_list_css(meta)
  local header_includes
  if meta['header-includes'] and meta['header-includes'].t == 'MetaList' then
    header_includes = meta['header-includes']
  else
    header_includes = pandoc.MetaList{meta['header-includes']}
  end
  header_includes[#header_includes + 1] =
    pandoc.MetaBlocks{pandoc.RawBlock('html', M.css_styles)}
  meta['header-includes'] = header_includes
  return meta
end

--- Replace the todo marker in the given block, if any.
function M.replace_todo_markers (blk, format)
  if blk == nil then
    return blk;
  end
  if blk.t ~= 'Para' and blk.t ~= 'Plain' then
    return blk
  end
  local inlines = blk.content
  local box, num_inlines = M.todo_marker(inlines, format)
  if box == nil then
    return blk
  end
  local new_inlines = List:new{box}
  for j = 1, #inlines do
    new_inlines[j + 1] = inlines[j + num_inlines]
  end
  return pandoc[blk.t](new_inlines) -- create Plain or Para
end

--- Convert Github- and org-mode-style task markers in a BulletList.
function M.modifyBulletList (list)
  if not is_html(FORMAT) then
    for _, item in ipairs(list.content) do
      item[1] = M.replace_todo_markers(item[1], FORMAT)
    end
    return list
  else
    local res = List:new{pandoc.RawBlock('html', '<ul>')}
    for _, item in ipairs(list.content) do
      local blk = M.replace_todo_markers(item[1], FORMAT)
      if blk == item[1] then -- does not have a todo marker
        res[#res + 1] = pandoc.RawBlock('html', '<li>')
      else
        res[#res + 1] = pandoc.RawBlock('html', '<li class="task-list-item">')
        item[1] = blk
      end
      res:extend(item)
      res[#res + 1] = pandoc.RawBlock('html', '</li>')
    end
    res[#res + 1] = pandoc.RawBlock('html', '</ul>')
    return res
  end
end

M[1] = {
  BulletList = M.modifyBulletList,
  Meta = is_html(FORMAT) and M.add_task_list_css or nil
}

return M
