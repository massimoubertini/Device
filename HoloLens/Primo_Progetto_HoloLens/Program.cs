using System;
using Windows.ApplicationModel.Core;
using Urho;
using Urho.Actions;
using Urho.SharpReality;
using Urho.Shapes;

namespace HelloWorld
{
    internal class Program
    {
        [MTAThread]
        private static void Main() => CoreApplication.Run(new UrhoAppViewSource<HelloWorldApplication>(new ApplicationOptions("Data")));
    }

    public class HelloWorldApplication : StereoApplication
    {
        private Node earthNode;

        public HelloWorldApplication(ApplicationOptions opts) : base(opts)
        {
        }

        protected override void Start()
        {
            // crea una scena di base
            base.Start();
            // crea un nodo per  Earth
            earthNode = Scene.CreateChild();
            // un metro di distanza
            earthNode.Position = new Vector3(0, 0, 1);
            earthNode.SetScale(0.2f); // 20cm
            // crea un componente del modello statico - Sphere:
            var earth = earthNode.CreateComponent<Sphere>();
            /* i materiali sono in genere più complicati delle semplici trame ma per i casi così
             * semplici si può usare il metodo FromImage per creare un materiale da un'immagine  */
            earth.SetMaterial(Material.FromImage("Textures/Earth.jpg"));
            // stessi passi per  Moon
            var moonNode = earthNode.CreateChild();
            // dimensioni relative Moon sono 1738.1km/6378.1km
            moonNode.SetScale(0.27f);
            moonNode.Position = new Vector3(1.2f, 0, 0);
            var moon = moonNode.CreateComponent<Sphere>();
            moon.SetMaterial(Material.FromImage("Textures/Moon.jpg"));
            // eseguire un'azione per far girare Earth (5 gradi al secondo)
            earthNode.RunActions(new RepeatForever(new RotateBy(duration: 1f, deltaAngleX: 0, deltaAngleY: -5, deltaAngleZ: 0)));
        }

        // per la stabilizzazione ottica HL (opzionale)
        public override Vector3 FocusWorldPoint => earthNode.WorldPosition;
    }
}