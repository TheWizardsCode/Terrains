using UnityEngine;
using UnityEngine.UI;

namespace WizardsCode.Materials
{
    /// <summary>
    /// The MaterialMetaData captures information about a material in the database.
    /// </summary>
    [CreateAssetMenu(fileName = "Material Meta Data", menuName = "Wizards Code/Meta Data/Material   ")]
    public class MaterialMetaData : ScriptableObject
    {
        public enum Category { Uncategorized, Other, Grass, Rock, Sand };

        public Category m_category = Category.Uncategorized;

        public Material m_Material;

        public class MetaDataOptionData : Dropdown.OptionData
        {
            internal MaterialMetaData data;

            public MetaDataOptionData(MaterialMetaData data)
            {
                this.data = data;
                text = data.name;
            }
        }

        public class CategoryOptionData : Dropdown.OptionData
        {
            internal Category m_category;

            public CategoryOptionData(Category category)
            {
                this.m_category = category;
                text = category.ToString();
            }
        }
    }
}
