namespace Match_3
{
    public partial class MainMenu : Form
    {

        public MainMenu()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Game game = new(this, 8, 8);
            game.Show();
            game.Run();
            this.Hide();
        }
    }


}