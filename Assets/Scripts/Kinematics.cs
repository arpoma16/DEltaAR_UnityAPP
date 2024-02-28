using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematics 
{
    // robot geometry
    //(look at pics above for explanation)
    const float e = 0.200f;     // Longitud del lado del triángulo equilátero pequeño (el que se moverá).
    const float f = 0.2755f;     // Longitud del lado del triángulo equilátero grande  (el que estará estático).
    const float re =0.348f;    // Longitud del brazo que va unido al triángulo pequeño y a 'rf'.348 antes 0.3385 -0.35
    const float rf = 0.1756f;    // Longitud del pequeño brazo que está unido al eje del servo y a 're'.178

    // trigonometric constants
    private static float sqrt3 = Mathf.Sqrt(3.0f);
    private const float pi = 3.141592653f;    // Pi
    private static float sin120 = Mathf.Sqrt(3.0f) / 2.0f;
    private const float cos120 = -0.5f;
    private static float tan60 = Mathf.Sqrt(3.0f);
    private const float sin30 = 0.5f;
    private static float tan30 = 1f / Mathf.Sqrt(3.0f);

    // Forward kinematics: (theta1, theta2, theta3) -> (x0, y0, z0)
    // Returned {error code,theta1,theta2,theta3}
    public static Vector4 Forward(float theta1, float theta2, float theta3)
    {
        float x0 = 0.0f;
        float y0 = 0.0f;
        float z0 = 0.0f;
        float t = (f - e) * tan30 / 2.0f;
        float dtr = pi / 180.0f;

        theta1 *= dtr;
        theta2 *= dtr;
        theta3 *= dtr;
        float y1 = -(t + rf * Mathf.Cos(theta1));
        float z1 = -rf * Mathf.Sin(theta1);

        float y2 = (t + rf * Mathf.Cos(theta2)) * sin30;
        float x2 = y2 * tan60;
        float z2 = -rf * Mathf.Sin(theta2);

        float y3 = (t + rf * Mathf.Cos(theta3)) * sin30;
        float x3 = -y3 * tan60;
        float z3 = -rf * Mathf.Sin(theta3);

        float dnm = (y2 - y1) * x3 - (y3 - y1) * x2;

        float w1 = y1 * y1 + z1 * z1;
        float w2 = x2 * x2 + y2 * y2 + z2 * z2;
        float w3 = x3 * x3 + y3 * y3 + z3 * z3;

        //x = (a1*z + b1)/dnm
        float a1 = (z2 - z1) * (y3 - y1) - (z3 - z1) * (y2 - y1);
        float b1 = -((w2 - w1) * (y3 - y1) - (w3 - w1) * (y2 - y1)) / 2.0f;

        //y = (a2*z + b2)/dnm
        float a2 = -(z2 - z1) * x3 + (z3 - z1) * x2;
        float b2 = ((w2 - w1) * x3 - (w3 - w1) * x2) / 2.0f;

        // a*z^2 + b*z + c = 0
        float a = a1 * a1 + a2 * a2 + dnm * dnm;
        float b = 2.0f * (a1 * b1 + a2 * (b2 - y1 * dnm) - z1 * dnm * dnm);
        float c = (b2 - y1 * dnm) * (b2 - y1 * dnm) + b1 * b1 + dnm * dnm * (z1 * z1 - re * re);

        // discriminant
        float d = b * b - 4.0f * a * c;
        if (d < 0.0f)
        {
            return new Vector4(1, 0, 0, 0);// non-existing povar. return error,x,y,z
        }
        else
        {
            z0 = -0.5f * (b + Mathf.Sqrt(d)) / a;
            x0 = (a1 * z0 + b1) / dnm;
            y0 = (a2 * z0 + b2) / dnm;
            return new Vector4(0, x0, y0, z0);
        }



    }

    // Inverse kinematics
    // Helper functions, calculates angle theta1 (for YZ-pane)

    public static Vector2 Angleyz(float x0, float y0, float z0)
    {
        float theta = 0;
        float y1 = -0.5f * 0.57735f * f; // f/2 * tg 30
        y0 -= 0.5f * 0.57735f * e; // shift center to edge
                                   // z = a + b*y
        float a = (x0 * x0 + y0 * y0 + z0 * z0 + rf * rf - re * re - y1 * y1) / (2.0f * z0);
        float b = (y1 - y0) / z0;

        //discriminant
        float d = -(a + b * y1) * (a + b * y1) + rf * (b * b * rf + rf);
        if (d < 0)
        {
            return new Vector2(1, 0);
        }
        else
        {
            float yj = (y1 - a * b - Mathf.Sqrt(d)) / (b * b + 1);// choosing outer povar
            float zj = a + b * yj;

            //theta = Mathf.Atan(-zj / (y1 - yj)) * 180.0 / pi + (180.0f if yj > y1 else 0.0);
            //theta = 180.0 * atan(-zj / (y1 - yj)) / pi + ((yj > y1) ? 180.0 : 0.0);
            // Si se cumple que yj>y1 entonces suma "180.0", de lo contrario no suma nada "0.0".
            theta = Mathf.Atan(-zj / (y1 - yj)) * 180.0f / pi + ((yj > y1) ? 180.0f : 0.0f);
            return new Vector2(0, theta);// return error, theta
        }




    }


    public static Vector4 Inverse(float x0, float y0, float z0)
    {
        float theta1 = 0;
        float theta2 = 0;
        float theta3 = 0;
        Vector2 status = Angleyz(x0, y0, z0);

        if (status.x == 0)
        {
            theta1 = status.y;
            status = Angleyz(x0 * cos120 + y0 * sin120, y0 * cos120 - x0 * sin120, z0);
        }
        if (status.x == 0)
        {
            theta2 = status.y;
            status = Angleyz(x0 * cos120 - y0 * sin120, y0 * cos120 + x0 * sin120, z0);
        }
        theta3 = status.y;
        return new Vector4(status.x, theta1, theta2, theta3);
    }


    public static Vector4 AngleBrazoInf(float AngleBrazoSup, float x0, float y0, float z0)
    {
        AngleBrazoSup = AngleBrazoSup * (pi / 180f);
        float y1 =- 0.5f * 0.57735f * f - rf * Mathf.Cos(AngleBrazoSup); // f/2 * tg 30
        float z1 =- rf * Mathf.Sin(AngleBrazoSup );
        float x1 = 0f;
        float zaux = 0f;
        float haux = 0f;

        y0 =y0 - 0.5f * 0.57735f * e;
        float Angle0 = Mathf.Atan((y0 - y1) / (z0 - z1)) * (180f / pi);
        haux = Mathf.Sqrt(Mathf.Pow(y0 - y1, 2)  + Mathf.Pow(z0 - z1, 2) );
        float Angle1 = Mathf.Atan((x0 - x1) / haux) * (180f / pi);

        zaux =re - re * Mathf.Cos(Angle1 * (pi / 180f));

        return new Vector4(Angle0, Angle1,x0, zaux);
    }
}
