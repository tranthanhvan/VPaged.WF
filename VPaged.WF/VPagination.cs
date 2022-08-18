using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VPaged.WF.Interfaces;
using VPaged.WF.VComponents;
using VPaged.WF.VProperties;

namespace VPaged.WF
{
    public class VPagination<TForm> where TForm : Form
    {
        private TForm _instanceImplement;
        private int _PageIndex { get; set; }
        private int _PageSize { get; set; }
        private double _TotalPage { get; set; }
        private List<VButton> _Buttons { get; set; }
        private Func<Task> _SelectDataMaster { get; set; }
        private ButtonStyle _ButtonStyle { get; set; }
        private Control _ContainerPagination { get; set; }
        private bool _StartWhenIntialize { get; set; }

        /// <summary>
        /// Function handler data
        /// </summary>
        public bool StartWhenIntialize
        {
            get
            {
                return _StartWhenIntialize;
            }
            set
            {
                _StartWhenIntialize = value;
            }
        }

        /// <summary>
        /// button style
        /// </summary>
        public ButtonStyle ButtonStyle
        {
            get
            {
                return _ButtonStyle;
            }
        }

        /// <summary>
        /// Function handler data
        /// </summary>
        public Func<Task> SelectDataMaster
        {
            get
            {
                return _SelectDataMaster;
            }
            set
            {
                _SelectDataMaster = value;
            }
        }

        /// <summary>
        /// Current page
        /// </summary>
        public int PageIndex
        {
            get
            {
                return _PageIndex;
            }
            set
            {
                _PageIndex = value;
            }
        }

        /// <summary>
        /// Number record in 1 page
        /// </summary>
        public int PageSize
        {
            get
            {
                return _PageSize;
            }
            set
            {
                _PageSize = value;
            }
        }

        public VPagination(TForm form, int pageIndex = 1, int pageSize = 15, Func<Task> handlerData = null, ButtonStyle style = null,bool startWhenIntialize = false)
        {
            _StartWhenIntialize = startWhenIntialize;
            _ButtonStyle = style ?? new ButtonStyle();
            this.InitializeComponent();
            _PageIndex = pageIndex;
            _PageSize = pageSize;
            _SelectDataMaster = handlerData;
            _Buttons = new List<VButton>()
            {
                this.btnPage1,
                this.btnPage2,
                this.btnPage3,
                this.btnPage4,
                this.btnPage5
            };
            _instanceImplement = form;
            this.InitializerEventHandler();
        }

        private void InitializerEventHandler()
        {
            this.numericPage.MouseDown -= new MouseEventHandler(this.numericPage_MouseDown);
            this.numericPage.MouseDown += new MouseEventHandler(this.numericPage_MouseDown);
            this.numericPage.MouseUp -= new MouseEventHandler(this.numericPage_MouseUp);
            this.numericPage.MouseUp += new MouseEventHandler(this.numericPage_MouseUp);

            this.btnPage1.Click -= new EventHandler(this.btnPage1_Click);
            this.btnPage1.Click += new EventHandler(this.btnPage1_Click);

            this.btnPage4.Click -= new EventHandler(this.btnPage4_Click);
            this.btnPage4.Click += new EventHandler(this.btnPage4_Click);

            this.btnGoPage.Click -= new EventHandler(this.btnGoPage_Click);
            this.btnGoPage.Click += new EventHandler(this.btnGoPage_Click);

            this.BtnFirstPage.Click -= new EventHandler(this.BtnFirstPage_Click);
            this.BtnFirstPage.Click += new EventHandler(this.BtnFirstPage_Click);

            this.btnLastPage.Click -= new EventHandler(this.btnLastPage_Click);
            this.btnLastPage.Click += new EventHandler(this.btnLastPage_Click);

            this.btnPage5.Click -= new EventHandler(this.btnPage5_Click);
            this.btnPage5.Click += new EventHandler(this.btnPage5_Click);

            this.btnPage2.Click -= new EventHandler(this.btnPage2_Click);
            this.btnPage2.Click += new EventHandler(this.btnPage2_Click);

            this.btnPage3.Click -= new EventHandler(this.btnPage3_Click);
            this.btnPage3.Click += new EventHandler(this.btnPage3_Click);

            _instanceImplement.Load -= new EventHandler(this.FormImplement_Load);
            _instanceImplement.Load += new EventHandler(this.FormImplement_Load);
        }

        /// <summary>
        /// Run pagination when implement use startWhenIntialize is false or refresh data paging
        /// </summary>
        public async void VPagRunOrRefresh()
        {
            await _SelectDataMaster();
            Button activeButton = _Buttons.Where(c => c.Text.Equals(_PageIndex.ToString())).FirstOrDefault();
            if (activeButton == null)
                HandlerPage(btnPage1);
            else
                HandlerPage(activeButton);
        }

        /// <summary>
        /// Pagination with TotalRecord use like column in collection
        /// </summary>
        /// <typeparam name="TData">TData ensure have Total property</typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="request">Request pagination.Contains pageIndex & pageSize</param>
        /// <param name="selector">Select display property</param>
        /// <param name="dataGridview">Datagridview reference</param>
        public void Pagination<TData, TRequest>(IEnumerable<TData> datas, TRequest request,
            Func<TData, object> selector, ref DataGridView dataGridview) where TRequest : IVPaginationRequest where TData : class, ITotalModel
        {
            dataGridview.DataSource = datas.Select(selector).ToList();
            PropertiesPagination pagination = this.PaginationGeneric<TData>(datas, request);
            //Set globals Total page
            _TotalPage = pagination.TotalPage;
            numericPage.Maximum = (decimal)_TotalPage;
            DisplayPaginationContainer();
        }

        /// <summary>
        /// Pagination with TotalPage use like params
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="totalPage">Total page</param>
        /// <param name="dataGridview">Datagridview reference</param>
        public void Pagination<TData>(IEnumerable<TData> datas, double totalPage, ref DataGridView dataGridview)
        {
            try
            {
                dataGridview.DataSource = datas;
                _TotalPage = totalPage;
                numericPage.Maximum = (decimal)_TotalPage;
                DisplayPaginationContainer();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Pagination
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="datas"></param>
        /// <param name="totalRecord"></param>
        /// <param name="dataGridview"></param>
        public void Pagination<TData>(IEnumerable<TData> datas,long totalRecord, ref DataGridView dataGridview)
        {
            try
            {
                dataGridview.DataSource = datas;

                double paging = Convert.ToDouble(totalRecord / _PageSize);
                paging = (totalRecord % _PageSize == 0 ? paging : paging + 1);
                paging = Math.Round(paging, MidpointRounding.AwayFromZero);

                _TotalPage = paging;
                numericPage.Maximum = (decimal)_TotalPage;
                DisplayPaginationContainer();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private PropertiesPagination PaginationGeneric<T>(IEnumerable<T> data, IVPaginationRequest request) where T : class, ITotalModel
        {
            if (data == null || !data.Any())
            {
                return new PropertiesPagination
                {
                    PageIndex = 0,
                    TotalPage = 0
                };
            }
            else
            {
                long Total = data.FirstOrDefault().Total;
                //paging
                var paging = Convert.ToDouble(Total / request.pageSize);
                paging = (Total % request.pageSize == 0 ? paging : paging + 1);

                paging = Math.Round(paging, MidpointRounding.AwayFromZero);
                return new PropertiesPagination
                {
                    PageIndex = request.pageIndex,
                    TotalPage = paging
                };
            }
        }

        private void HandlerPage(Button btnClick)
        {
            if (_TotalPage == 0)
                return;
            //Rewrite
            _PageIndex = int.Parse(btnClick.Text);
            if (_PageIndex > _TotalPage)
                _PageIndex = (int)_TotalPage;
            double centerPage = _PageIndex;
            double rangeMinNew = centerPage - 2;
            double rangeMaxNew = centerPage + 2;

            if (rangeMinNew < 1)
            {
                double lostLast = 1 - rangeMinNew;
                rangeMinNew = 1;
                rangeMaxNew = rangeMaxNew + lostLast;
            }

            if (rangeMaxNew > _TotalPage)
            {
                rangeMaxNew = _TotalPage;
            }

            btnPage1.Text = rangeMinNew.ToString();
            btnPage2.Text = (rangeMinNew + 1).ToString();
            btnPage3.Text = (rangeMinNew + 2).ToString();
            btnPage4.Text = (rangeMinNew + 3).ToString();
            btnPage5.Text = (rangeMinNew + 4).ToString();

            //First display all
            btnPage1.Visible = true;
            btnPage2.Visible = true;
            btnPage3.Visible = true;
            btnPage4.Visible = true;
            btnPage5.Visible = true;

            Button activeButton = _Buttons.Where(c => c.Text.Equals(_PageIndex.ToString())).FirstOrDefault();
            if (activeButton != null)
            {
                activeButton.BackColor = ButtonStyle.ColorActive;
                activeButton.Enabled = false;
            }

            Button itemUnActiveFront = _Buttons.Where(c => !c.Text.Equals(_PageIndex.ToString()) && c.BackColor == ButtonStyle.ColorActive).FirstOrDefault();
            if (itemUnActiveFront != null)
            {
                itemUnActiveFront.BackColor = ButtonStyle.BackColor;
                itemUnActiveFront.Enabled = true;
            }

            //Filter display
            double countPageRange = rangeMaxNew - rangeMinNew;
            if (countPageRange < 4)
            {
                if (countPageRange == 3)
                {
                    btnPage5.Visible = false;
                }
                if (countPageRange == 2)
                {
                    btnPage4.Visible = false;
                    btnPage5.Visible = false;
                }
                if (countPageRange == 1)
                {
                    btnPage3.Visible = false;
                    btnPage4.Visible = false;
                    btnPage5.Visible = false;
                }
                if (countPageRange == 0)
                {
                    btnPage2.Visible = false;
                    btnPage3.Visible = false;
                    btnPage4.Visible = false;
                    btnPage5.Visible = false;
                }
            }

            if (_PageIndex == 1)
            {
                BtnFirstPage.Enabled = false;
            }
            else
            {
                BtnFirstPage.Enabled = true;
            }

            if (_PageIndex == _TotalPage)
            {
                btnLastPage.Enabled = false;
            }
            else
            {
                btnLastPage.Enabled = true;
            }

            this.numericPage.Value = _PageIndex;
            if (_instanceImplement != null && _SelectDataMaster != null)
            {
                _SelectDataMaster();
            }
            else
            {
                throw new Exception("Instance implement or select function master is not intializer");
            }
            //End
        }

        private void btnPage1_Click(object sender, EventArgs e)
        {
            HandlerPage(btnPage1);
        }

        private void btnPage2_Click(object sender, EventArgs e)
        {
            HandlerPage(btnPage2);
        }

        private void btnPage3_Click(object sender, EventArgs e)
        {
            HandlerPage(btnPage3);
        }

        private void btnPage4_Click(object sender, EventArgs e)
        {
            HandlerPage(btnPage4);
        }

        private void btnPage5_Click(object sender, EventArgs e)
        {
            HandlerPage(btnPage5);
        }

        private void BtnFirstPage_Click(object sender, EventArgs e)
        {
            btnPage1.Text = "1";
            HandlerPage(btnPage1);
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            btnPage5.Text = _TotalPage.ToString();
            HandlerPage(btnPage5);
        }

        private async void FormImplement_Load(object sender, EventArgs e)
        {
            numericPage.Minimum = 1;
            if (_StartWhenIntialize)
            {
                await _SelectDataMaster();
                HandlerPage(btnPage1);
            }
            else
                _ContainerPagination.Visible = false;
        }

        private void btnGoPage_Click(object sender, EventArgs e)
        {
            decimal pageSelect = numericPage.Value;
            btnPage3.Text = pageSelect.ToString();
            HandlerPage(btnPage3);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnPage1 = new VButton(_ButtonStyle);
            this.btnPage4 = new VButton(_ButtonStyle);
            this.btnPage3 = new VButton(_ButtonStyle);
            this.btnPage2 = new VButton(_ButtonStyle);
            this.btnPage5 = new VButton(_ButtonStyle);
            this.btnLastPage = new VButton(_ButtonStyle);
            this.BtnFirstPage = new VButton(_ButtonStyle);
            this.btnGoPage = new VButton(_ButtonStyle);
            this.numericPage = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericPage)).BeginInit();
            //
            // btnPage1
            //
            this.btnPage1.Name = "btnPage1";
            this.btnPage1.TabIndex = 1;
            this.btnPage1.Text = "1";
            //
            // btnPage2
            //
            this.btnPage2.Name = "btnPage2";
            this.btnPage2.TabIndex = 2;
            this.btnPage2.Text = "2";
            //
            // btnPage3
            //
            this.btnPage3.Name = "btnPage3";
            this.btnPage3.TabIndex = 3;
            this.btnPage3.Text = "3";
            //
            // btnPage4
            //
            this.btnPage4.Name = "btnPage4";
            this.btnPage4.TabIndex = 4;
            this.btnPage4.Text = "4";
            //
            // btnPage5
            //
            this.btnPage5.Name = "btnPage5";
            this.btnPage5.TabIndex = 5;
            this.btnPage5.Text = "5";
            //
            // btnLastPage
            //
            this.btnLastPage.Name = "btnLastPage";
            this.btnLastPage.TabIndex = 6;
            this.btnLastPage.Text = ">>";
            //
            // BtnFirstPage
            //
            this.BtnFirstPage.Name = "BtnFirstPage";
            this.BtnFirstPage.TabIndex = 0;
            this.BtnFirstPage.Text = "<<";
            //
            // btnGoPage
            //
            this.btnGoPage.Name = "btnGoPage";
            this.btnGoPage.TabIndex = 8;
            this.btnGoPage.Text = "->";

            //
            // numericPage
            //
            this.numericPage.Name = "numericPage";
            this.numericPage.TabIndex = 7;
            //
            // FrmPagination
            //
            ((System.ComponentModel.ISupportInitialize)(this.numericPage)).EndInit();
        }

        #endregion Windows Form Designer generated code

        protected VButton btnPage1;
        protected VButton btnPage4;
        protected VButton btnPage3;
        protected VButton btnPage2;
        protected VButton btnPage5;
        protected VButton btnLastPage;
        protected VButton BtnFirstPage;
        protected VButton btnGoPage;
        protected NumericUpDown numericPage;

        private void numericPage_MouseUp(object sender, MouseEventArgs e)
        {
            decimal pageSelect = numericPage.Value;
            btnPage3.Text = pageSelect.ToString();
            HandlerPage(btnPage3);
        }

        private void numericPage_MouseDown(object sender, MouseEventArgs e)
        {
            decimal pageSelect = numericPage.Value;
            btnPage3.Text = pageSelect.ToString();
            HandlerPage(btnPage3);
        }

        /// <summary>
        /// Display pagition in screen
        /// </summary>
        /// <param name="container">Group box</param>
        public void GetPaginationUI(Control container)
        {
            if (_ContainerPagination != null)
                return;

            if (container.Height < 48)
                container.Height = 48;
            if (container.Width < 370)
                container.Width = 370;
            container.Controls.Add(this.BtnFirstPage);
            container.Controls.Add(this.btnPage1);
            container.Controls.Add(this.btnPage2);
            container.Controls.Add(this.btnPage3);
            container.Controls.Add(this.btnPage4);
            container.Controls.Add(this.btnPage5);
            container.Controls.Add(this.btnLastPage);
            container.Controls.Add(this.numericPage);
            container.Controls.Add(this.btnGoPage);
            fitControlsToScreen(container);
            container.SuspendLayout();
            container.ResumeLayout(false);
            _ContainerPagination = container;
        }

        private void DisplayPaginationContainer()
        {
            if (_TotalPage <= 1)
                _ContainerPagination.Visible = false;
            else
                _ContainerPagination.Visible = true;
        }

        private void fitControlsToScreen(Control container)
        {
            #region info Container

            int widtdContainer = container.Width;
            int heightContainer = container.Height;

            #endregion info Container

            int marginOtherBtn = 5;
            int paddingLeft = 17
                , paddingRight = 17;

            int btnHeight = heightContainer * 65 / 100;

            int marginTopBotBtn = (heightContainer - btnHeight) / 2;

            int widthCanUse = (widtdContainer - paddingLeft - paddingRight);

            int partPageNumber = widthCanUse * 70 / 100;

            int btnWidth = (partPageNumber - (marginOtherBtn * 6)) / 7;

            var controlPartPages = new List<Control>()
            {
                this.BtnFirstPage,
                this.btnPage1,
                this.btnPage2,
                this.btnPage3,
                this.btnPage4,
                this.btnPage5,
                this.btnLastPage
            };

            var controlPartGoToPages = new List<Control>()
            {
                this.numericPage,
                this.btnGoPage
            };

            Control previous = null;
            foreach (Control item in container.Controls)
            {
                if (controlPartPages.Contains(item))
                {
                    item.Size = new Size(btnWidth, btnHeight);
                    if (previous != null)
                        item.Location = new System.Drawing.Point(previous.Location.X + previous.Size.Width + marginOtherBtn, previous.Location.Y);
                    else
                        item.Location = new Point(paddingLeft, marginTopBotBtn + 3);
                    previous = item;
                }
                else if (controlPartGoToPages.Contains(item))
                {
                    if (item == this.numericPage)
                    {
                        int widthNumeric = btnWidth * 3 / 2;
                        item.Width = widthNumeric;
                        int heightNumeric = item.Height;

                        int deviant = (previous.Height - heightNumeric) / 2;

                        item.Location = new Point(widtdContainer - paddingRight - btnWidth - marginOtherBtn - widthNumeric, marginTopBotBtn + 3 + deviant);
                    }
                    else if (item == this.btnGoPage)
                    {
                        item.Size = new Size(btnWidth, btnHeight);
                        item.Location = new Point(widtdContainer - paddingRight - btnWidth, marginTopBotBtn + 3);
                        previous = item;
                    }
                }
            }
        }
    }
}