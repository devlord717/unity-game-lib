// Version 1.4
// ©2011 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Graphics.Vector {

    [AddComponentMenu("Vectrosity/LineManager")]
    public class LineManager : GameObjectBehavior {
        private static List<VectorLine> lines;
        private static List<Transform> transforms;
        private static int lineCount = 0;
        private bool destroyed = false;

        private void Awake() {
            lines = new List<VectorLine>();
            transforms = new List<Transform>();
            DontDestroyOnLoad(this);
        }

        public void AddLine(VectorLine vectorLine, Transform thisTransform, float time) {
            if (time > 0.0f) {	// Needs to be before the line check, to accommodate re-added lines
                StartCoroutine(DisableLine(vectorLine, time, true));
            }
            for (int i = 0; i < lineCount; i++) {
                if (vectorLine == lines[i]) {
                    return;
                }
            }
            lines.Add(vectorLine);
            transforms.Add(thisTransform);

            if (++lineCount == 1) {
                enabled = true;
            }
        }

        public void DisableLine(VectorLine vectorLine, float time) {
            StartCoroutine(DisableLine(vectorLine, time, false));
        }

        private IEnumerator DisableLine(VectorLine vectorLine, float time, bool remove) {
            yield return new WaitForSeconds(time);
            if (remove) {
                RemoveLine(vectorLine);
            }
            Vector.DestroyLine(ref vectorLine);
        }

        private void LateUpdate() {
            if (!Vector.camTransformExists) return;

            // Only redraw if camera is moving
            if (Vector.oldPosition != Vector.camTransformPosition || Vector.oldRotation != Vector.camTransformEulerAngles) {
                for (int i = 0; i < lineCount; i++) {
                    if (lines[i].vectorObject != null) {
                        Vector.DrawLine3D(lines[i], transforms[i]);
                    }
                    else {
                        RemoveLine(i--);
                    }
                }

                VectorManager.DrawArrayLines();
            }

            Vector.oldPosition = Vector.camTransformPosition;
            Vector.oldRotation = Vector.camTransformEulerAngles;

            // Always redraw dynamic objects
            VectorManager.DrawArrayLines2();
        }

        private void RemoveLine(int i) {
            lines.RemoveAt(i);
            transforms.RemoveAt(i);
            --lineCount;
            DisableIfUnused();
        }

        public void DisableIfUnused() {
            if (!destroyed) { // Prevent possible null reference exceptions
                if (lineCount == 0 && VectorManager.arrayCount == 0 && VectorManager.arrayCount2 == 0) {
                    enabled = false;
                }
            }
        }

        public void EnableIfUsed() {
            if (VectorManager.arrayCount == 1 || VectorManager.arrayCount2 == 1) {
                enabled = true;
            }
        }

        public void RemoveLine(VectorLine vectorLine) {
            for (int i = 0; i < lineCount; i++) {
                if (vectorLine == lines[i]) {
                    RemoveLine(i);
                    break;
                }
            }
        }

        public void StartCheckDistance() {
            InvokeRepeating("CheckDistance", .01f, VectorManager.distanceCheckFrequency);
        }

        private void CheckDistance() {
            VectorManager.CheckDistance();
        }

        private void OnDestroy() {
            destroyed = true;
        }
    }
}