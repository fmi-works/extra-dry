﻿#nullable enable

namespace ExtraDry.Core
{

    public interface IBlobInfo
    {

        public Guid UniqueId { get; set; }

        public string Filename { get; set; }

        public string Url { get; set; }

        [MaxLength(256)]
        public string MimeType { get; set; }
    }

}
