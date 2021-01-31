using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Gradient", 12)]
public class UI_Gradient : MaskableGraphic
{
    [HideInInspector]
    [SerializeField]
    private Texture m_GeneratedTexture;

    [SerializeField]
    private UnityEngine.Gradient m_Gradient;
    [SerializeField]
    private bool m_Invert;

    public override Texture mainTexture
    {
        get
        {
            if (m_GeneratedTexture == null)
            {
                if (m_Gradient == null)
                {
                    m_GeneratedTexture = s_WhiteTexture;
                }

                m_GeneratedTexture = GradientUtils.CreateTexture(m_Gradient, (int)rectTransform.rect.width, (int)rectTransform.rect.height, m_Invert);
            }

            return m_GeneratedTexture;
        }
    }

    public UnityEngine.Gradient gradient
    {
        get { return m_Gradient; }
        set
        {
            if (m_Gradient.Equals(value)) return;

            m_Gradient = value;
            SetVerticesDirty();
            SetMaterialDirty();
        }
    }

    public override void SetMaterialDirty()
    {
        base.SetMaterialDirty();
        m_GeneratedTexture = GradientUtils.CreateTexture(m_Gradient, (int)rectTransform.rect.width, (int)rectTransform.rect.height, m_Invert);
    }
}
