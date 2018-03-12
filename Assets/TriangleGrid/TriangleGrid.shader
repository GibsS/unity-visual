Shader "Experiment/TriangleGrid"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

        _Width("Width", float) = 50
        _Height("Height", float) = 50
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float4 gridPos : TEXCOORD1;
                float2 texcoord : TEXCOORD0;
            };
            
            fixed4 _Color;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif
                OUT.gridPos = IN.vertex;

                return OUT;
            }

            sampler2D _MainTex;
            float _AlphaSplitEnabled;

            float _Width;
            float _Height;

            fixed4 frag(v2f IN) : SV_Target
            {
                // FIND POSITION IN SQUARE
                float2 pos;
                pos.x = IN.gridPos.x;
                pos.y = IN.gridPos.z;

                // CALCULATED XY POSITION IN GRID + XY POSITION WITHIN CELL
                float2 relPos = frac(pos);

                pos = floor(pos);

                pos.x += 0.5;
                pos.y += 0.5;

                float2 pos2 = float2((_Width + pos.x) / (2 * _Width), pos.y / _Height);

                pos.x /= 2 * _Width;
                pos.y /= _Height;

                fixed4 c1 = tex2D (_MainTex, pos);
                fixed4 c2 = tex2D (_MainTex, pos2);

                // FOR ANTI ALIASING ("BLURS" THE HYPOTENUS OF TRIANGLES)
                float border = 0.005;

                // PICK DEPENDING ON CASE
                if(c1.a < 0.1) { // EMPTY
                    return fixed4(0, 0, 0, 0);
                } else if(c1.a < 0.2) { // FULL
                    c1.a = 1;
                    return c1;
                } else if(c1.a < 0.3) { // BOTTOM LEFT
                    float t = relPos.y - 1 + relPos.x;

                    t = smoothstep(-border, border, t);

                    c1.a = 1;

                    return lerp(c1, c2, t);
                } else if(c1.a < 0.4) { // BOTTOM RIGHT
                    float t = relPos.y - relPos.x;

                    t = smoothstep(-border, border, t);

                    c1.a = 1;

                    return lerp(c1, c2, t);
                } else if(c1.a < 0.5) { // TOP LEFT
                    float t = relPos.y - relPos.x;

                    t = smoothstep(-border, border, t);

                    c1.a = 1;

                    return lerp(c2, c1, t);
                } else { // TOP RIGHT
                    float t = relPos.y - 1 + relPos.x;

                    t = smoothstep(-border, border, t);

                    c1.a = 1;

                    return lerp(c2, c1, t);
                }
            }


        ENDCG
        }
    }
}