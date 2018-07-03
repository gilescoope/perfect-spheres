#if UNITY_EDITOR
using UnityEditor.ShaderGraph;
using System.Reflection;
using UnityEngine;

[Title("Sphere", "Sphere Normal")]
public class SphereNormalNode : CodeFunctionNode {
    public SphereNormalNode() {
        name = "Sphere Normal";
    }

    protected override MethodInfo GetFunctionToConvert() {
        return GetType().GetMethod("SphereNormal", BindingFlags.Static | BindingFlags.NonPublic);
    }

    static string SphereNormal([Slot(0, Binding.None)] Vector3 Position, [Slot(1, Binding.None)] Vector3 Direction, [Slot(2, Binding.None)] Vector1 Radius, [Slot(3, Binding.None)] out Vector3 Normal, [Slot(4, Binding.None)] out Vector1 Alpha)
    {
        Normal = Vector3.zero;
        return @"
{
    float DD = dot(Direction, Direction);
    float DP = dot(Direction, Position);
    float PP = dot(Position, Position);
    float det2 = DP*DP-DD*(PP - Radius*Radius);
	Alpha = saturate(det2/fwidth(det2));
    float t = (-DP - sqrt(det2))/DD;
    Normal = (Position + t*Direction)/Radius;
}
";
    }
}

[Title("Sphere", "Sphere UV")]
public class SphereUVNode : CodeFunctionNode {
    public SphereUVNode() {
        name = "Sphere UV";
    }

    protected override MethodInfo GetFunctionToConvert() {
        return GetType().GetMethod("SphereUV", BindingFlags.Static | BindingFlags.NonPublic);
    }

    static string SphereUV([Slot(0, Binding.None)] Vector3 Position, [Slot(1, Binding.None)] out Vector2 UV1, [Slot(2, Binding.None)] out Vector2 UV2, [Slot(3, Binding.None)] out Vector1 Alpha)
    {
        UV1 = Vector2.zero;
        UV2 = Vector2.zero;
        return @"
{
    float theta = 0.5 + 0.15915494309 * atan2(Position.z, Position.x);
    UV1.x = theta;
    UV1.y = 0.5 + 0.31830988618 * asin(Position.y);
    UV2.x = fmod(theta + 0.5, 1); - 0.5;
    UV2.y = UV1.y;
    Alpha = 2 * abs(theta - 0.5);
}
";
    }
}
#endif