using System;

namespace Contract
{
	public class UploadDocumentContext
    {
        public Guid DocumentPublicId { get; set; }
        public Guid TransactionId { get; set; }
        public int CustomerId { get; set; }
        public string FilePath { get; set; }
        public FileLocation FileLocation { get; set; }
        public FileUploadSteps FileUploadStatus { get; set; }
    }
}
