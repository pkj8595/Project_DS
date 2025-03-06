using UnityEngine;
using Coffee.UIExtensions;

public class UIParticleTest : MonoBehaviour
{
    public ParticleSystem ps;
    public UIParticle uIParticle;
    public UIParticleAttractor atttactor;

    public short count = 1;
    public int type = 1;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var em = ps.emission;
            em.SetBurst(0, new ParticleSystem.Burst(0f, count,count,1,0.01f));

            ps.Play();

        }
    }
}
