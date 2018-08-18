﻿using AView = Android.Views;
using Android.Runtime;
using Android.Views;
using Xamarin.Forms;
using System.ComponentModel;
using Android.Content;
using Android.Renderscripts;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.RadialMenu;
using Xamarin.Forms.RadialMenu.AndroidCore;
using Xamarin.Forms.RadialMenu.Models;

[assembly: ExportRenderer(typeof(RadialMenu), typeof(DraggableMenuRenderer))]
namespace Xamarin.Forms.RadialMenu.AndroidCore
{

    public class DraggableMenuRenderer : VisualElementRenderer<Xamarin.Forms.View>
    {
        float originalX;
        float originalY;
        float dX;
        float dY;
        bool firstTime = true;
        bool touchedDown = false;
        bool hasmoved = false;
        public DraggableMenuRenderer(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);
            

            if (e.OldElement != null)
            {
                LongClick -= HandleLongClick;
            }
            if (e.NewElement != null)
            {
                LongClick += HandleLongClick;
                var dragView = Element as RadialMenu;
                Click += DraggableMenuRenderer_Click;

                //dragView.RestorePositionCommand = new Command(() =>
                //{
                //    if (!firstTime)
                //    {
                //        SetX(originalX);
                //        SetY(originalY);
                //    }

                //});
            }

        }

        private void DraggableMenuRenderer_Click(object sender, System.EventArgs e)
        {
            var dragView = Element as RadialMenu;
            if (!dragView.IsOpened)
            {
                dragView.OpenMenu();
            }
        }

        private void HandleLongClick(object sender, LongClickEventArgs e)
        {
            var dragView = Element as RadialMenu;
            if (firstTime)
            {
                originalX = GetX();
                originalY = GetY();
                firstTime = false;
            }
            dragView.DragStarted();
            touchedDown = true;
        }

        protected override void OnVisibilityChanged(AView.View changedView, [GeneratedEnum] ViewStates visibility)
        {
            base.OnVisibilityChanged(changedView, visibility);
            if (visibility == ViewStates.Visible)
            {

            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            float x = e.RawX;
            float y = e.RawY;
            var dragView = Element as RadialMenu;
            //if (!dragView.IsOpened)
            //{
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    if (dragView.DragMode == RadialMenu.DragMod.Touch)
                    {
                        if (!touchedDown)
                        {
                            if (firstTime)
                            {
                                originalX = GetX();
                                originalY = GetY();
                                firstTime = false;
                            }

                            dragView.DragStarted();
                        }

                        touchedDown = true;
                    }

                    dX = x - this.GetX();
                    dY = y - this.GetY();

                    break;
                case MotionEventActions.Move:
                    if (touchedDown)
                    {
                        if (dragView.DragDirection == RadialMenu.DragDirectionType.All ||
                            dragView.DragDirection == RadialMenu.DragDirectionType.Horizontal)
                        {
                            SetX(x - dX);
                        }

                        if (dragView.DragDirection == RadialMenu.DragDirectionType.All ||
                            dragView.DragDirection == RadialMenu.DragDirectionType.Vertical)
                        {
                            SetY(y - dY);
                        }
                        hasmoved = true;
                    }

                    break;
                case MotionEventActions.Up:
                    touchedDown = false;
                    dragView.DragEnded();
                    if (hasmoved)
                    {
                        dragView.IsOpened = false;
                        hasmoved = false;
                    }

                    break;
                case MotionEventActions.Cancel:
                    touchedDown = false;
                    break;
                    //}
            }

            return base.OnTouchEvent(e);
        }

        public override bool OnInterceptTouchEvent(MotionEvent e)
        {
            var dragView = Element as RadialMenu;

            BringToFront();
            if (dragView.IsOpened)
                return false;
            return true;

        }
    }

}
