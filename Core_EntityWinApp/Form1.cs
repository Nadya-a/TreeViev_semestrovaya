using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Core_EntityWinApp
{
    public partial class Form1 : Form
    {
        private TContext _context;
        public Form1()
        {
            InitializeComponent();
            treeView1.BeforeExpand += TreeView1_BeforeExpand;
            FormClosing += Form1_FormClosing;
            _context = new TContext("NewConnection");
            InitRootGroup(1);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _context.Database.Connection.Close();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void TreeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            //Получаемраскрываемыйузел
            //var expandedNode = e.Node;
            TreeNode node = e.Node;
            if (node==null)
            {
                MessageBox.Show($@"Раскрываемый узел пустой!");
                return;
            }
            int id = Int32.Parse(node.Name.Split('|')[0]);
            node.Nodes.Clear();
            foreach (var group in _context.TGROUP_EntityProperty.Where(group =>
                _context.TRELATION_EntityProperty.Where(relation => relation.ParentId == id).Select(relation => relation.ChildId).Contains(group.TGroupId)))
            {
                TreeNode child = new TreeNode() { Text = group.TGROUPname, Name = group.TGroupId + "|TGROUP" };
                child.Nodes.Add(new TreeNode() { Text = "TechnicalGroup", Name = "0|TechnicalGroup" });
                node.Nodes.Add(child);
            }

            foreach (var property in _context.TPROPERTY_EntityProperty.Where(property => property.TGroupId == id))
            {
                TreeNode child = new TreeNode() { Text = property.TPropertyName+": "+property.TPropertyValue, Name = property.TPropertyId + "|TPROPERTY" };
                //TreeNode child = new TreeNode() { Text = property.TPropertyName, Name = property.TPropertyId + "|TPROPERTY" };
                //TreeNode Value = new TreeNode() { Text = property.TPropertyValue, Name = property.TPropertyId + "|TPROPERTY" };
                node.Nodes.Add(child);
                //child.Nodes.Add(Value);
                return;
            }
        }

        //private void группуToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    TreeNode node = treeView1.SelectedNode;
        //    int id = Int32.Parse(node.Name.Split('|')[0]);
        //    string type = node.Name.Split('|')[1];
        //    if (type.Equals("TGROUP"))
        //    {
        //        string name = "----";
        //        int newId = CreateNewTGroup(name);
        //        CreateNewTRelation(id, newId);
        //        TreeNode child = new TreeNode() { Text = name, Name = newId + "|TGROUP" };
        //        child.Nodes.Add(new TreeNode() { Text = "TechnicalGroup", Name = "0|TechnicalGroup" });
        //        node.Nodes.Add(child);
        //    }
        //}
        private void InitRootGroup(int id)
        {
            var readGroup = _context.TGROUP_EntityProperty.FirstOrDefault(x => x.TGroupId == 1);
            if (readGroup == null)
            {
                MessageBox.Show($@"Объект с id = {id} не найден, отсутствует корневая группа!");
                //return;
            }
            string text = _context.TGROUP_EntityProperty.FirstOrDefault(x => x.TGroupId == 1).TGROUPname;
            var rootNode = new TreeNode()
            {
                Text = text,
                Name = "1|TGroup"
            };
            var techNode = new TreeNode()
            {
                Text = "TechnicalGroup",
                Name = "TechnicalGroup"
            };
            rootNode.Nodes.Add(techNode);
            treeView1.Nodes.Add(rootNode);
        }

        private int CreateNewTGroup(string name)
        {
            int maxId = 1;
            // Получаем значение параметра id для нового объекта сущности
            try
            {
                maxId = _context.TGROUP_EntityProperty.Max(x => x.TGroupId) + 1;
            }
            catch(Exception e) { }
            //Создается экземпляр класса TGROUPclass
            var newTGroup = new TGROUPclass()
            {
                TGroupId = maxId,
                TGROUPname = name
            };
            // Происходитдобавлениезаписивтаблицу "TestTable"
            _context.TGROUP_EntityProperty.Add(newTGroup);
            return maxId;
        }

        private void DeleteTGroup(int id)
        {
            if (_context == null) return;
            // Из базы данных находится объект с заданным параметров id
            var TGroupForDelete = _context.TGROUP_EntityProperty.FirstOrDefault(x => x.TGroupId == id);
            if (TGroupForDelete == null)
            {
                MessageBox.Show($@"Объект с id = {id} не найден!");
                return;
            }
            _context.TGROUP_EntityProperty.Remove(TGroupForDelete);
        }

        private void UpdateTGroupName(int id, string name)
        {
            if (_context == null) return;
            // Находится из базы данных объект по параметру id, у которого необходимо изменить свойства TGROUPname.
            var TGroupForUpdate = _context.TGROUP_EntityProperty.FirstOrDefault(x => x.TGroupId == id);
            if (TGroupForUpdate == null)
            {
                MessageBox.Show(@"Объект, предназначенный для обновления, не найден");
                return;
            }
            // Происходит изменение значение свойства TGROUPname на значение переменной name
            TGroupForUpdate.TGROUPname = name;
        }

        private void CreateNewTRelation(int parent_id, int child_id)
        {
            if (_context == null) return;
            //var maxId = _context.TRELATION_EntityProperty.Max(x => x.TRelationId) + 1;
            var newTRelation = new TRELATIONclass()
            {
                ParentId = parent_id,
                ChildId = child_id
            };
            _context.TRELATION_EntityProperty.Add(newTRelation);
        }

        private void DeleteTRelation(int parent_id, int child_id)
        {
            if (_context == null) return;
            var TRelationForDelete1 = _context.TRELATION_EntityProperty.SingleOrDefault(x => x.ParentId == parent_id);
            var TRelationForDelete2 = _context.TRELATION_EntityProperty.SingleOrDefault(y => y.ChildId == child_id);
            if (TRelationForDelete1 == null && TRelationForDelete2 == null)
            {
                MessageBox.Show($@"Объект {parent_id}, {child_id} не найден!");
                return;
            }
            //??????????????????????
            _context.TRELATION_EntityProperty.Remove(TRelationForDelete1);
            _context.TRELATION_EntityProperty.Remove(TRelationForDelete2);
        }

        private void UpdateTRelation(int parent_id, int child_id, int NEWparent_id, int NEWchild_id)
        {
            if (_context == null) return;
            var TRelationForUpdate1 = _context.TRELATION_EntityProperty.SingleOrDefault(x => x.ParentId == parent_id);
            var TRelationForUpdate2 = _context.TRELATION_EntityProperty.SingleOrDefault(y => y.ChildId == child_id);
            if (TRelationForUpdate1 == null && TRelationForUpdate2 == null)
            {
                MessageBox.Show($@"Объект {parent_id}, {child_id} не найден!");
            }
            TRelationForUpdate1.ParentId = NEWparent_id;
            TRelationForUpdate2.ChildId = NEWchild_id;
        }

        private int CreateNewTProperty(string name, string value, int group_id)
        {
            int maxId = 1;
            // Получаем значение параметра id для нового объекта сущности
            try
            {
                maxId = _context.TPROPERTY_EntityProperty.Max(x => x.TPropertyId) + 1;
            }
            catch (Exception e) { }
            var newTProperty = new TPROPERTYClass()
            {
                TPropertyId = maxId,
                TPropertyName = name,
                TPropertyValue = value,
                TGroupId = group_id
            };
            _context.TPROPERTY_EntityProperty.Add(newTProperty);
            return maxId;
        }

        private void DeleteTProperty(int id)
        {
            if (_context == null) return;
            // Из базы данных находится объект с заданным параметров id
            var TPropertyForDelete = _context.TPROPERTY_EntityProperty.SingleOrDefault(x => x.TPropertyId == id);
            if (TPropertyForDelete == null)
            {
                MessageBox.Show($@"Объект с id = {id} не найден!");
                return;
            }
            _context.TPROPERTY_EntityProperty.Remove(TPropertyForDelete);
        }

        private void UpdateTProperty(int id, string name, string value/*, int group_id*/)
        {
            if (_context == null) return;
            var TPropertyForUpdate = _context.TPROPERTY_EntityProperty.FirstOrDefault(x => x.TPropertyId == id);
            if (TPropertyForUpdate == null)
            {
                MessageBox.Show(@"Объект, предназначенный для обновления, не найден");
                return;
            }
            TPropertyForUpdate.TPropertyName = name;
            TPropertyForUpdate.TPropertyValue = value;
            //TPropertyForUpdate.TGroupId = group_id;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void группуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            int id = Int32.Parse(node.Name.Split('|')[0]);
            string type = node.Name.Split('|')[1];
            if (type.Equals("TGROUP"))
            {
                string name = "----";
                int newId = CreateNewTGroup(name);
                CreateNewTRelation(id, newId);
                TreeNode child = new TreeNode() { Text = name, Name = newId + "|TGROUP" };
                child.Nodes.Add(new TreeNode() { Text = "TechnicalGroup", Name = "0|TechnicalGroup" });
                node.Nodes.Add(child);
            }
        }

        private void свойствоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            int id = Int32.Parse(node.Name.Split('|')[0]);
            string type = node.Name.Split('|')[1];
            if (type.Equals("TGROUP"))
            {
                string name = "----";
                string value = "----";
                int newId = CreateNewTProperty(name, value, id);
                TreeNode child = new TreeNode() { Text = name, Name = newId + "|TPROPERTY" };

                node.Nodes.Add(child);
            }
        }

        private void редактироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            string type = node.Name.Split('|')[1];
            int id = Int32.Parse(node.Name.Split('|')[0]);
            if (type.Equals("TGROUP"))
            {
                groupBox1.Enabled = true;
                textBox1.Text = node.Text;
                textBox2.Text = id.ToString();

                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
            }
            else
            {
                groupBox2.Enabled = true;
                textBox1.Clear();
                textBox2.Clear();

                TPROPERTYClass property = _context.TPROPERTY_EntityProperty.FirstOrDefault(prop => prop.TPropertyId == id);

                textBox3.Text = property.TPropertyName;
                textBox4.Text = property.TPropertyValue;
                textBox5.Text = property.TGroupId.ToString();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            string type = node.Name.Split('|')[1];
            int id = Int32.Parse(node.Name.Split('|')[0]);
            if (type.Equals("TGROUP"))
            {
                DeleteTGroup(id);
            }
            else
            {
                DeleteTProperty(id);
            }
            node.Parent.Nodes.Remove(node);
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            int id = Int32.Parse(node.Name.Split('|')[0]);
            UpdateTGroupName(id, textBox1.Text);
            node.Text = textBox1.Text;
            textBox1.Clear();
            textBox2.Clear();
            groupBox1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            int id = Int32.Parse(node.Name.Split('|')[0]);
            UpdateTProperty(id, textBox3.Text, textBox4.Text);
            node.Text = textBox3.Text;
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            groupBox2.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
        }
    }
}

[Table("TGROUP")]
public class TGROUPclass
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("id")]
    public int TGroupId { get; set; }
    [Column("name")]
    public string TGROUPname { get; set; }
}

[Table("TRELATION")]
public class TRELATIONclass
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("id_parent", Order = 1)]
    public int ParentId { get; set; }
    [Key]
    [Column("id_child", Order = 2)]
    public int ChildId { get; set; }
}

[Table("TPROPERTY")]
public class TPROPERTYClass
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("id")]
    public int TPropertyId { get; set; }
    [Column("name")]
    public string TPropertyName { get; set; }
    [Column("value")]
    public string TPropertyValue { get; set; }
    [Column("group_id")]
    public int TGroupId { get; set; }
}

public partial class TContext : DbContext
{
    public TContext(string conneсtionName) : base(conneсtionName)
    {
    }
    public virtual DbSet<TGROUPclass> TGROUP_EntityProperty { get; set; }
    public virtual DbSet<TRELATIONclass> TRELATION_EntityProperty { get; set; }
    public virtual DbSet<TPROPERTYClass> TPROPERTY_EntityProperty { get; set; }
}

