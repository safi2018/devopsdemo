using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using I95Dev.Connector.UI.Base.DataAccess;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Helpers.Controls;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.Services;
using I95Dev.Connector.UI.Base.ViewModels.Base;

namespace I95Dev.Connector.UI.Base.ViewModels.Configuration
{
    public class DocumentTypesViewModel : BaseViewModel
    {
        #region Properties

        private string validationMessage;
        private bool showPopup;
        private DocumentType documentType;
        private readonly IDocumentTypeDataAccess localSql;
        private DocumentType selectedDocumentId;

        public string ValidationMessage
        {
            get { return validationMessage; }
            private set { SetProperty(ref validationMessage, value); }
        }

        public bool ShowPopup
        {
            get { return showPopup; }
            set
            {
                if (!SetProperty(ref showPopup, value)) return;
                ValidationMessage = string.Empty;
                StatusUpdate.IsPopUpOpen = value;
            }
        }

        public ObservableRangeCollection<DocumentType> Responses { get; }

        public DocumentType DocumentType
        {
            get { return documentType; }
            set { SetProperty(ref documentType, value); }
        }

        public DocumentType SelectedDocumentId
        {
            get { return selectedDocumentId; }
            set { SetProperty(ref selectedDocumentId, value); }
        }

        public IList<DocumentType> ComboBoxGpDocuments { get; }

        #endregion Properties

        public DocumentTypesViewModel()
        {
            localSql = new DocumentTypeDataAccess();

            bool result = localSql.IsDocumentTypeTableExists();
            if (!result)
            {
                NotificationHelper.ShowMessage("DocumentType table not found", Utilities.GetResourceValue("CaptionError"));
                return;
            }

            Responses = new ObservableRangeCollection<DocumentType>();
            AddNewTypeCommand = new BaseCommand(AddNewType);
            ViewRecordCommand = new BaseCommand(ViewRecord);
            ClosePopupCommand = new BaseCommand(ClosePopup);
            SaveDocumentTypeCommand = new BaseCommand(SaveRecord);

            ComboBoxGpDocuments = localSql.LoadDocumentTypes();
            LoadGrid();
        }

        #region Commands

        public ICommand AddNewTypeCommand { get; }

        private void AddNewType()
        {
            ShowPopup = true;
            DocumentType = new DocumentType();
            ValidationMessage = "";
        }

        public ICommand ViewRecordCommand { get; }

        private void ViewRecord(object arg)
        {
            if (!(arg is DocumentType model)) return;

            DocumentType = model;
            ShowPopup = true;
            ValidationMessage = "";
        }

        public ICommand ClosePopupCommand { get; }

        private void ClosePopup()
        {
            ShowPopup = false;
        }

        public ICommand SaveDocumentTypeCommand { get; }

        private void SaveRecord()
        {
            try
            {
                ValidationMessage = "All fields are required";
                if (SelectedDocumentId == null) return;
                if (string.IsNullOrEmpty(DocumentType.Orgin) || string.IsNullOrEmpty(DocumentType.ErpDocumentId) || SelectedDocumentId.SopType <= 0) return;

                DocumentType.SopType = SelectedDocumentId.SopType;

                if (localSql.CheckDocumentTypeCombination(DocumentType))
                {
                    ValidationMessage = "Combination Already Exist";
                    return;
                }

                if (localSql.SaveDocumentTypeDetails(DocumentType))
                {
                    ShowPopup = false;
                    NotificationHelper.ShowMessage("Updated Successfully", Utilities.GetResourceValue("CaptionInfo"));
                    ValidationMessage = "";
                    LoadGrid();
                }
                else
                {
                    ValidationMessage = "Unknown error occured. Please try again after some time";
                }
            }
            catch (Exception exception)
            {
                ValidationMessage = exception.Message;
            }
        }

        #endregion Commands

        #region Methods

        private void LoadGrid()
        {
            IList<DocumentType> records = localSql.GetDocumentMappings();
            Responses.Clear();
            if (!records.Any()) return;
            Responses.AddRange(records);
        }

        #endregion Methods
    }
}