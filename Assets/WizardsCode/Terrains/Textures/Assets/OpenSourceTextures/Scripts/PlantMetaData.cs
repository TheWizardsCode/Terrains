using UnityEngine;
using UnityEngine.UI;

namespace WizardsCode.Texture { 
    /// <summary>
    /// The MaterialMetaData captures information about a material in the database.
    /// </summary>
    [CreateAssetMenu(fileName = "Texture Meta Data", menuName = "Wizards Code/Meta Data/Texture")]
    public class PlantMetaData : ScriptableObject
    {
        public enum Category { Uncategorized, Other, Grass, Flower, Weed, Cereal };

        public Category m_category = Category.Uncategorized;

        public UnityEngine.Texture m_Texture;

        public class MetaDataOptionData : Dropdown.OptionData
        {
            internal PlantMetaData data;

            public MetaDataOptionData(PlantMetaData data)
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
