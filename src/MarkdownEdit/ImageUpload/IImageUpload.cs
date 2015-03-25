// Copyright (c) 2015 Blue Onion Software - All rights reserved

using System.Threading.Tasks;

namespace MarkdownEdit.ImageUpload
{
    public interface IImageUpload
    {
        Task<string> UploadBytesAsync(byte[] imageBytes);
    }
}