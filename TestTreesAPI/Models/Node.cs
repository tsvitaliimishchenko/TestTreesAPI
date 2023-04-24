using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace TestTreesAPI.Models
{
    public class Node
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [JsonIgnore]
        public int? ParentNodeId { get; set; }

        [JsonIgnore]
        public string TreeName { get; set; }

        [JsonIgnore] 
        public virtual Node Parent { get; set; }

        public ICollection<Node> Children { get; set; } = new List<Node>();

        public Node(string name, string treeName, int? parentNodeId, bool isRoot)
        {
            Name = name;
            ParentNodeId = parentNodeId;
            TreeName = treeName;
        }

        private Node()
        { }
    }
}