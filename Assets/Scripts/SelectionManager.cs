using Assets.Scripts.HexLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class SelectionManager
    {
        private GameObject selectedObject;
        /// <summary>
        /// Storing reference to selected object
        /// if no object is selected will return null
        /// </summary>
        public GameObject SelectedObject
        {
            get
            {
                return selectedObject;
            }
            set
            {
                if (selectedObject != value)   //clicking second time disables selection
                    selectedObject = value;
                else
                    selectedObject = null;

                //Debug.Log(String.Format("Selected grid cell: {0}",
                //    _selectedObject != null ? _selectedObject.tag : "no object selected")
                //    );


            }
        }

        /// <summary>
        /// Storing reference to selected grid cell
        /// if no cell is selected will return null
        /// </summary>
        private HexCell gridCellSelection;
        public HexCell GridCellSelection
        {
            get
            {
                return gridCellSelection;
            }
            set
            {
                if (gridCellSelection != value)   //clicking second time disables selection
                    gridCellSelection = value;
                else
                    gridCellSelection = null;

                //Debug.Log( String.Format("Selected grid cell: {0}",
                //   _gridCellSelection != null ? _gridCellSelection.coordinates.ToString() : "no grid selected" )
                //   ); 
            }
        }
    }
}
