using Fusion;
using UnityEngine;

namespace Starter.Platformer
{
    public class MovingPlatform : NetworkBehaviour
    {
        [Header("Setup")]
        public Transform destino1;
        public Transform destino2;
        public float velocidad = 1.0f;

        [Header("References")]
        public Rigidbody Platform;
        public Transform visual; // Referencia al modelo visual de la plataforma

        [Networked]
        private NetworkBool _isMovingToDestino1 { get; set; } = true;

        [Networked]
        private Vector3 _networkedPosition { get; set; }

        private Vector3 posDestino1;
        private Vector3 posDestino2;

        private void Start()
        {
            if (Platform == null || destino1 == null || destino2 == null || visual == null)
            {
                Debug.LogError("Faltan referencias en MovingPlatform");
                return;
            }

            Platform.isKinematic = false; // Asegura que el Rigidbody no sea kinemático

            // Guardamos las posiciones iniciales de los destinos
            posDestino1 = destino1.position;
            posDestino2 = destino2.position;
        }

        public override void FixedUpdateNetwork()
        {
            if (Platform == null) return;

            Vector3 objetivoActual = _isMovingToDestino1 ? posDestino1 : posDestino2;

            if (Vector3.Distance(Platform.position, objetivoActual) < 0.1f)
            {
                _isMovingToDestino1 = !_isMovingToDestino1;
                objetivoActual = _isMovingToDestino1 ? posDestino1 : posDestino2;
            }

            Vector3 newPosition = Vector3.MoveTowards(Platform.position, objetivoActual, velocidad * Runner.DeltaTime);
            Platform.MovePosition(newPosition);

            // Mueve manualmente el modelo visual si no es hijo del Rigidbody
            visual.position = newPosition;

            // Sincroniza la posición de la plataforma en todos los clientes
            _networkedPosition = newPosition;
        }

        public override void Render()
        {
            // Asegúrate de que la posición visual se actualice correctamente
            visual.position = _networkedPosition;
        }
    }
}