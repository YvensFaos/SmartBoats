using System;

[Serializable]
public class TagWeight
{
    public string tag;
    public float weight;

    public TagWeight(string tag, float weight)
    {
        this.tag = tag;
        this.weight = weight;
    }
}
