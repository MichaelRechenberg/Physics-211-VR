using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    /// <summary>
    /// Class containing common Measures used in PHYS 211 labs
    /// 
    /// A Measure obeys the following signature:
    /// 
    ///     public delegate double Measure(Rigidbody rb, Dictionary<string, double> args);
    ///     
    /// </summary>
    class Measures
    {
        /// <summary>
        /// Calculates the Kinetic Energy of the RigidBody
        /// </summary>
        /// <param name="rb">The Rigidbody to extract the velocity from</param>
        /// <param name="args">An empty Dictionary</param>
        /// <returns></returns>
        public static double KineticEnergy(Rigidbody rb, Dictionary<string, double> args)
        {
            return 0.5 * rb.mass * Mathf.Pow(rb.velocity.magnitude, 2);
        }

        /// <summary>
        /// Calculates the Gravitational Potential Energy of the Rigidbody
        /// 
        /// Requires the following keys to be defined in args:
        ///     * "gravity" the acceleration due to gravity: m/s^2
        ///     * "height" the height of the Rigid body from what is considered the ground: m
        /// </summary>
        /// <param name="rb">The Rigidbody to extract the mass from</param>
        /// <param name="args">A Dictionary with the key/value pairs outlined above</param>
        /// <returns></returns>
        public static double GravitationalPotentialEnergy(Rigidbody rb, Dictionary<string, double> args)
        {
            return rb.mass * args["gravity"] * args["height"];
        }
    }
}
