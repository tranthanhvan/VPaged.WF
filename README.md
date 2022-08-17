# VPaged.WF Pagination component for winform base
This library will help you to paginate with winform easily.

# How to use

### Simple example : Initialize a VPag with the current Form ,set method you use to get data in SelectDataMaster and run:
```csharp
    public partial class Form1 : Form
    {
        private VPagination<Form1> _pag;
        public Form1()
        {
            InitializeComponent();
            _pag = new VPagination<Form1>(this); 
            _pag.PageSize = 10;
            _pag.GetPaginationUI(this.paginationContainer); //Get display pager. in this example 'this.paginationContainer' is a GroupBox
            _pag.SelectDataMaster = () => SelectDataMaster(); // Function select data eg below:
        }
        
        public async Task SelectDataMaster()
        {
            try
            {
                var data = Employee.GetAllEmployees().Skip((_pag.PageIndex - 1) * _pag.PageSize)
                                        .Take(_pag.PageSize).ToList();

                int count = Employee.GetAllEmployees().Count();
                _pag.Pagination(data, (c) => new
                {
                    ID = c.ID,
                    Name = c.Name
                }, (long)count, _pag.PageSize, ref this.GridMaster); // this.GridMaster is DataGridView.
            }
            catch (Exception ex)
            {
               throw ex;
            }
        }
    }
```





