

    using System.Collections.Generic;

    public static class TagWeightHelper
    {
        public static List<TagWeight> GenerateDefaultTagWeightList()
        {
            List<TagWeight> list = new List<TagWeight>();
            string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
            float weight = 0.0f;
            foreach (string tag in tags)
            {
                list.Add(new TagWeight(tag, weight));
            }

            return list;
        }
    }
