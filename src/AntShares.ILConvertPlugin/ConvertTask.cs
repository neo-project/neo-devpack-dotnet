namespace AntShares
{

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    /// <summary>
    /// this class generate config / xml file by template
    /// </summary>
    public class ConvertTask : Task
    {
        [Required]
        public ITaskItem DataSource { get; set; }

        /// <summary>
        /// execute replace logic
        /// </summary>
        /// <returns>ture successful, false failure</returns>
        public override bool Execute()
        {
            this.Log.LogWarning("come on baby.");
            this.Log.LogWarning("src=" + this.DataSource.ToString());

            return true;
        }
    }

}