namespace Contract
{
	public enum FileUploadSteps
    {
        NotStarted = 0,
        AcquireFiles = 1,
        CreateSQLRelation = 2,
        UploadDocumentsInStore = 3,
        CreateTheSupportingStructureInMongoDb = 4
    }
}
