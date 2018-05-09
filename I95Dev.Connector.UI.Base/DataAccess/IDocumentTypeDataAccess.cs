using System.Collections.Generic;
using I95Dev.Connector.UI.Base.Models;

namespace I95Dev.Connector.UI.Base.DataAccess
{
    internal interface IDocumentTypeDataAccess
    {
        bool IsDocumentTypeTableExists();

        IList<DocumentType> LoadDocumentTypes();

        IList<DocumentType> GetDocumentMappings();

        bool CheckDocumentTypeCombination(DocumentType documentType);

        bool SaveDocumentTypeDetails(DocumentType documentType);
    }
}