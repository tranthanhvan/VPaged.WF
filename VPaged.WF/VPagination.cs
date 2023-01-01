using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VPaged.WF.VComponents;
using VPaged.WF.VProperties;

namespace VPaged.WF
{
    public class VPagination
    {
        private int _PageIndex { get; set; }

        private int _PageSize { get; set; }

        private double _TotalPage { get; set; }

        private List<VButton> _Buttons { get; set; }

        private Action _SelectDataMaster { get; set; }

        private Func<long> _SelectCountMaster { get; set; }

        private ButtonStyle _ButtonStyle { get; set; }

        private Control _ContainerPagination { get; set; }

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
        public Action SelectDataMaster
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
        /// Function get count
        /// </summary>
        public Func<long> SelectCountMaster
        {
            get
            {
                return _SelectCountMaster;
            }
            set
            {
                _SelectCountMaster = value;
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

        public double TotalPage
        {
            get => _TotalPage;
        }

        /// <summary>
        /// Initializer VPag
        /// </summary>
        /// <param name="containerDisplay"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="selectDataMethod">Method set data</param>
        /// <param name="selectCountMethod">Method get count record to paging</param>
        /// <param name="style">btn style define</param>
        public VPagination(Control containerDisplay, int pageIndex = 1, int pageSize = 15, Action selectDataMethod = null, Func<long> selectCountMethod = null, ButtonStyle style = null)
        {
            _ButtonStyle = style ?? new ButtonStyle();
            this.InitializeComponent();
            _PageIndex = pageIndex;
            _PageSize = pageSize;
            _SelectDataMaster = selectDataMethod;
            _SelectCountMaster = selectCountMethod;
            _Buttons = new List<VButton>()
            {
                this.btnPage1,
                this.btnPage2,
                this.btnPage3,
                this.btnPage4,
                this.btnPage5
            };
            numericPage.Minimum = 1;
            SetPaginationUI(containerDisplay);
            DisplayPaginationContainer();
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
        }


        /// <summary>
        /// this to help VPaged get the latest TotalPage .
        /// Limiting the second SelectMaster callback. it will calcuation & set totalPage based totalRecord
        /// </summary>
        /// <param name="totalRecord">total record need paging</param>
        private void SetTotalPage(long totalRecord)
        {
            if (totalRecord < 0)
                _TotalPage = 0;
            else
            {
                double paging = Convert.ToDouble(totalRecord / _PageSize);
                paging = (totalRecord % _PageSize == 0 ? paging : paging + 1);
                paging = Math.Round(paging, MidpointRounding.AwayFromZero);
                _TotalPage = paging;
            }
        } 

        /// <summary>
        /// Run pagination when implement use startWhenIntialize is false or refresh data paging
        /// </summary>
        public void VPagRunOrRefresh()
        {
            if(_SelectCountMaster is null)
                throw new Exception("Select Count function master is not intializer");

            SetTotalPage(_SelectCountMaster());
            Button activeButton = _Buttons.Where(c => c.Text.Equals(_PageIndex.ToString())).FirstOrDefault();
            if (activeButton == null)
                HandlerPage(btnPage1);
            else
                HandlerPage(activeButton);
        }

        /// <summary>
        /// Use this method to display data paging
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="datas"></param>
        /// <param name="dataGridview"></param>
        public void Pagination<TData>(IEnumerable<TData> datas, ref DataGridView dataGridview)
        {
            try
            {
                dataGridview.DataSource = datas;
                numericPage.Maximum = (decimal)_TotalPage;
                DisplayPaginationContainer();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Use this method to display data paging use DataTable
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="datas"></param>
        /// <param name="dataGridview"></param>
        public void Pagination(DataTable datas, ref DataGridView dataGridview)
        {
            try
            {
                dataGridview.DataSource = datas;
                numericPage.Maximum = (decimal)_TotalPage;
                DisplayPaginationContainer();
            }
            catch (Exception ex)
            {
                throw ex;
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
            if (_SelectDataMaster != null)
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
        private void SetPaginationUI(Control container)
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