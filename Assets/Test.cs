using UnityEngine;
using System.Collections;
using System.Numerics;
using System;

public class Test : MonoBehaviour {
    public ComplexRenderer complexRenderer;

	// Use this for initialization
	void Start () {
        Update();
    }

    // Update is called once per frame
    void Update () {
        //Test000();
        TestQuantumHarmonicOscillator(1);
    }

    private void Test000() {
        float time = Time.realtimeSinceStartup;
        int count = 10;
        Complex[] values = new Complex[count];
        for (int i = 0; i < count; i++) {
            values[i] = Complex.FromPolarCoordinates(1, time + (i * 2 * Mathf.PI / count));
        }

        complexRenderer.SetValues(values);
    }

    private static long Factorial(long n) {
        long result = 1;
        for (int i = 2; i <= n; i++) {
            result *= i;
        }
        return result;
    }

    private void TestQuantumHarmonicOscillator(int n) {
        double t = Time.realtimeSinceStartup;
        int count = 100;
        double scale = 10.0;
        // I wish I knew whether it was actually more efficient to make these constants out here
        //TODO I am suspicious of how
        double h1 = 10.0;
        double E1 = 1.0;
        Complex niE_h1 = -1 * (E1 / h1) * Complex.ImaginaryOne;
        double m1 = 1;
        double w1 = 1;
        double psi_a1 = Math.Sqrt(1.0 / (Math.Pow(2, n) * Factorial(n))) * Math.Pow((m1 * w1) / (Math.PI * h1), 0.25);
        double nmw_2h1 = -1 * m1 * w1 / (2 * h1);
        double sqrtmw_h1 = Math.Sqrt(m1 * w1 / h1);
        Complex[] values = new Complex[count];
        for (int i = 0; i < count; i++) {
            double x = ((i * scale) / count) - (scale / 2);
            //values[i] = Complex.FromPolarCoordinates(1, t + (i * 2 * Mathf.PI / count));
            Complex psi = psi_a1 * Math.Exp(nmw_2h1 * x * x) * Hermite(n, sqrtmw_h1 * x);
            values[i] = psi * Complex.Exp(t * niE_h1);
        }

        double h2 = 1.0;
        double E2 = 1.0;
        Complex niE_h2 = -1 * (E2 / h2) * Complex.ImaginaryOne;
        double m2 = 1;
        double w2 = 1;
        double psi_a2 = Math.Sqrt(1.0 / (Math.Pow(2, n) * Factorial(n))) * Math.Pow((m2 * w2) / (Math.PI * h2), 0.25);
        double nmw_2h2 = -1 * m2 * w2 / (2 * h2);
        double sqrtmw_h2 = Math.Sqrt(m2 * w2 / h2);
        for (int i = 0; i < count; i++) {
            double x = ((i * scale) / count) - (scale / 2);
            //values[i] = Complex.FromPolarCoordinates(1, t + (i * 2 * Mathf.PI / count));
            Complex psi = psi_a2 * Math.Exp(nmw_2h2 * x * x) * Hermite(n, sqrtmw_h2 * x);
            values[i] += psi * Complex.Exp(t * niE_h2);
        }

        complexRenderer.SetValues(values);
    }

    private static double Hermite(int n, double x) {
        switch (n) {
            case 0: return 1;
            case 1: return 2*x;
            case 2: return 4*x*x - 2;
            case 3: return 8*Math.Pow(x,3) - 12*x;
            default:
                throw new Exception("Hermite polynomial " + n + " not yet implemented");
        }
    }
}
