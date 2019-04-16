using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Web;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Controls;
using Kesco.Lib.Entities.Resources;
using Kesco.Lib.Log;
using Kesco.Lib.Web.Controls.V4;
using Kesco.Lib.Web.Controls.V4.Common;

namespace Kesco.App.Web.CashFlowItems
{
    /// <summary>
    ///     Вид движения денежных средств
    /// </summary>
    public partial class CashFlowTypeForm : EntityPage
    {
        protected CashFlowType cft;
        protected string id;
        protected string idDoc;
        protected string idParentPage;
        protected bool IsParentUpdate = true;

        /// <summary>
        ///     Задание ссылки на справку
        /// </summary>
        public override string HelpUrl { get; set; }

        /// <summary>
        ///     Событие загрузки страницы
        /// </summary>
        /// <param name="sender">Объект страницы</param>
        /// <param name="e">Аргументы</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!V4IsPostBack) V4SetFocus("efName");
        }

        /// <summary>
        ///     Валидация контролов
        /// </summary>
        /// <returns></returns>
        private bool Validation()
        {
            var title = Resx.GetString("Cfi_msgRecordCannotBeSaved");
            if (cft.Id.Length == 0 || cft.Id == "0")
            {
                // Не заполнено поле 'Код'"
                ShowMessage(Resx.GetString("Cfi_msgNotFilledField") + " «" + Resx.GetString("Cfi_lblCode") + "»",
                    title);
                efId.Focus();
                return false;
            }

            if (cft.Name.Length == 0)
            {
                // Не заполнено поле 'Название'"
                ShowMessage(Resx.GetString("Cfi_msgNotFilledField") + " «" + Resx.GetString("Cfi_lblName") + "»",
                    title);
                efName.Focus();
                return false;
            }

            if (cft.Name1С.Length == 0)
            {
                // Не заполнено поле 'Название 1C'"
                ShowMessage(Resx.GetString("Cfi_msgNotFilledField") + " «" + Resx.GetString("Cfi_lblNameIn1C") + "»",
                    title);
                efName1C.Focus();
                return false;
            }

            if (!efId.IsReadOnly)
                if (cft.ExistIdDubl())
                {
                    ShowMessage(Resx.GetString("Cfi_msgIdDubl") + " «" + cft.Id + "»", title);
                    efId.Focus();
                    return false;
                }

            return true;
        }

        /// <summary>
        ///     Подготовка данных для отрисовки заголовка страницы(панели с кнопками)
        /// </summary>
        /// <returns></returns>
        protected string RenderDocumentHeader()
        {
            using (var w = new StringWriter())
            {
                try
                {
                    ClearMenuButtons();
                    SetMenuButtons();
                    RenderButtons(w);
                }
                catch (Exception e)
                {
                    var dex = new DetailedException("Не удалось сформировать кнопки формы: " + e.Message, e);
                    Logger.WriteEx(dex);
                    throw dex;
                }

                return w.ToString();
            }
        }

        /// <summary>
        ///     Инициализация/создание кнопок меню
        /// </summary>
        private void SetMenuButtons()
        {
            var btnAdd = new Button
            {
                ID = "btnSave",
                V4Page = this,
                Text = Resx.GetString("cmdSave") + "&nbsp;(F2)",
                Title = Resx.GetString("cmdSave"),
                Width = 125,
                IconJQueryUI = ButtonIconsEnum.Save,
                OnClick = "cmd('cmd', 'SaveData');"
            };

            var btnRefresh = new Button
            {
                ID = "btnRefresh",
                V4Page = this,
                Text = Resx.GetString("cmdRefresh"),
                Title = Resx.GetString("cmdRefreshTitle"),
                IconJQueryUI = ButtonIconsEnum.Refresh,
                Width = 105,
                OnClick = "cmd('cmd', 'RefreshData');"
            };

            var btnDelete = new Button
            {
                ID = "btnDelete",
                V4Page = this,
                Text = Resx.GetString("cmdDelete"),
                Title = Resx.GetString("cmdDeleteTitle"),
                IconJQueryUI = ButtonIconsEnum.Delete,
                Width = 105,
                OnClick = string.Format("v4_showConfirm('{0}','{1}','{2}','{3}','{4}', null);",
                    string.Format("{0} «{1}»", Resx.GetString("msgDeleteConfirm"), HttpUtility.HtmlEncode(cft.Name)),
                    Resx.GetString("errDoisserWarrning"),
                    Resx.GetString("CONFIRM_StdCaptionYes"),
                    Resx.GetString("CONFIRM_StdCaptionNo"),
                    string.Format("cmd({0});", HttpUtility.JavaScriptStringEncode("'cmd', 'DeleteData'"))
                )
            };

            var btnClose = new Button
            {
                ID = "btnClose",
                V4Page = this,
                Text = Resx.GetString("cmdClose"),
                Title = Resx.GetString("cmdCloseTitleApp"),
                IconJQueryUI = ButtonIconsEnum.Close,
                Width = 105,
                OnClick = IsParentUpdate ? "parent.Records_Close(idp, null);" : "window.close();"
            };

            if (!cft.Id.IsNullEmptyOrZero())
            {
                Button[] buttons = {btnAdd, btnRefresh, btnDelete, btnClose};
                AddMenuButton(buttons);
            }
            else
            {
                Button[] buttons = {btnAdd, btnRefresh, btnClose};
                AddMenuButton(buttons);
            }
        }

        /// <summary>
        ///     инициализация контролов
        /// </summary>
        protected override void EntityFieldInit()
        {
            if (!V4IsPostBack)
            {
                id = Request.QueryString["id"];
                idParentPage = Request.QueryString["idpp"];

                ParentPage = Application[idParentPage] as EntityPage;
                if (ParentPage == null) IsParentUpdate = false;

                if (!string.IsNullOrEmpty(id) && id != "0")
                {
                    cft = new CashFlowType(id);
                    if (cft == null || cft.Id == "0")
                        throw new LogicalException(Resx.GetString("Cfi_ ERRMoveCashInitialized"), "",
                            Assembly.GetExecutingAssembly().GetName(), Priority.Info);
                }
                else
                {
                    cft = new CashFlowType {Id = id};
                }
            }

            Entity = cft;

            efId.BindStringValue = cft.CashFlowTypeIdBind;
            efName.BindStringValue = cft.CashFlowTypeNameBind;
            efName1C.BindStringValue = cft.CashFlowTypeName1CBind;

            efId.IsReadOnly = cft.Id != "0";

            base.EntityFieldInit();
        }

        /// <summary>
        ///     Обработка клиентских команд
        /// </summary>
        /// <param name="cmd">Команды</param>
        /// <param name="param">Параметры</param>
        protected override void ProcessCommand(string cmd, NameValueCollection param)
        {
            switch (cmd)
            {
                case "RefreshData":
                    JS.Write("$('#btnRefresh').attr('disabled', 'disabled');Wait.render(true);");
                    JS.Write(
                        "setTimeout(function(){{$('#btnRefresh').removeAttr('disabled'); Wait.render(false);}}, 2000);");
                    RefreshData();
                    break;
                case "SaveData":
                    SaveData();
                    break;
                case "DeleteData":
                    DeleteData();
                    break;
            }
        }

        /// <summary>
        ///     Кнопка: Сохранить
        /// </summary>
        private void SaveData()
        {
            if (Validation())
            {
                var reloadParentForm = true;
                var isNew = !efId.IsReadOnly;
                cft.Save(isNew);
                if (IsParentUpdate)
                    JS.Write("parent.Records_Save('','{0}','{1}');", reloadParentForm, isNew);
                else
                    JS.Write("window.close();");
            }
        }

        /// <summary>
        ///     Обновление данных формы из объекта
        /// </summary>
        private void RefreshData()
        {
            ClearCacheObjects();
            RefreshNtf();
        }

        /// <summary>
        ///     Очистка всех данных формы
        /// </summary>
        private void DeleteData()
        {
            cft.Delete();
            if (IsParentUpdate)
                JS.Write("parent.Records_Save();");
            else
                JS.Write("window.close();");
        }
    }
}