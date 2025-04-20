using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Verse
{
    // Token: 0x0200046B RID: 1131
    public struct RWPT_MaterialRequest : IEquatable<RWPT_MaterialRequest>
    {
        // Token: 0x170006D1 RID: 1745
        // (set) Token: 0x06002053 RID: 8275 RVA: 0x000C2B17 File Offset: 0x000C0D17
        public string BaseTexPath
        {
            set
            {
                this.mainTex = ContentFinder<Texture2D>.Get(value, true);
            }
        }

        public RWPT_MaterialRequest(MaterialRequest req, params Color[] colors)
        {
            this.shader = req.shader;
            this.mainTex = req.mainTex;
            this.renderQueue = req.renderQueue;
            this.maskTex = req.maskTex;
            this.shaderParameters = req.shaderParameters;
            this.needsMainTex = req.needsMainTex;
            this.colors = colors.Clone() as Color[];
        }
        public RWPT_MaterialRequest(Texture tex)
        {
            this.shader = ShaderDatabase.Cutout;
            this.mainTex = tex;
            this.colors = [Color.white, Color.white];
            this.maskTex = null;
            this.renderQueue = 0;
            this.shaderParameters = null;
            this.needsMainTex = true;
        }

        // Token: 0x06002055 RID: 8277 RVA: 0x000C2B7C File Offset: 0x000C0D7C
        public RWPT_MaterialRequest(Texture tex, Shader shader)
        {
            this.shader = shader;
            this.mainTex = tex;
            this.colors = [Color.white, Color.white];
            this.maskTex = null;
            this.renderQueue = 0;
            this.shaderParameters = null;
            this.needsMainTex = true;
        }

        public RWPT_MaterialRequest(Texture tex, Shader shader, Texture2D maskTex, params Color[] colors)
        {
            this.shader = shader;
            this.mainTex = tex;
            this.colors = colors.Clone() as Color[];
            this.maskTex = maskTex;
            this.renderQueue = 0;
            this.shaderParameters = null;
            this.needsMainTex = true;
        }

        public RWPT_MaterialRequest(Shader shader)
        {
            this.shader = shader;
            this.mainTex = null;
            this.colors = [Color.white, Color.white];
            this.maskTex = null;
            this.renderQueue = 0;
            this.shaderParameters = null;
            this.needsMainTex = false;
        }

        public override int GetHashCode()
        {
            var hash = Gen.HashCombine<List<ShaderParameter>>(Gen.HashCombineInt(Gen.HashCombine<Texture2D>(Gen.HashCombine<Texture>(Gen.HashCombine<Shader>(0, this.shader), this.mainTex), this.maskTex), this.renderQueue), this.shaderParameters);
            foreach(Color color in this.colors)
            {
                hash = Gen.HashCombine(hash, color);
            }
            return hash;
        }

        public override bool Equals(object obj)
        {
            return obj is RWPT_MaterialRequest && this.Equals((RWPT_MaterialRequest)obj);
        }

        public bool Equals(RWPT_MaterialRequest other)
        {
            return other.shader == this.shader && other.mainTex == this.mainTex && Enumerable.SequenceEqual(colors, other.colors) && other.maskTex == this.maskTex && other.renderQueue == this.renderQueue && other.shaderParameters == this.shaderParameters;
        }

        public static bool operator ==(RWPT_MaterialRequest lhs, RWPT_MaterialRequest rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(RWPT_MaterialRequest lhs, RWPT_MaterialRequest rhs)
        {
            return !(lhs == rhs);
        }

        public override string ToString()
        {
            return string.Concat(new string[]
            {
                "MaterialRequest(",
                this.shader?.name ?? "null",
                ", ",
                this.mainTex?.name ?? "null",
                ", ",
                string.Join(", ", colors),
                ", ",
                this.maskTex?.ToString() ?? "null",
                ", ",
                this.renderQueue.ToString(),
                ")"
            });
        }

        public Shader shader;

        public Texture mainTex;

        public Color[] colors;

        public Texture2D maskTex;

        public int renderQueue;

        public bool needsMainTex;

        public List<ShaderParameter> shaderParameters;
    }
}
