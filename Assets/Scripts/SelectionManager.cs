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
        private GameObject _selectedObject;
        /// <summary>
        /// Storing reference to selected object
        /// if no object is selected will return null
        /// </summary>
        public GameObject SelectedObject
        {
            get
            {
                return _selectedObject;
            }
            set
            {
                if (_selectedObject != value)   //clicking second time disables selection
                    _selectedObject = value;
                else
                    _selectedObject = null;

                Debug.Log(String.Format("Selected grid cell: {0}",
                    _selectedObject != null ? _selectedObject.tag : "no object selected")
                    );

                
            }
        }

        /// <summary>
        /// Storing reference to selected grid cell
        /// if no cell is selected will return null
        /// </summary>
        private HexCell _gridCellSelection;
        public HexCell GridCellSelection
        {
            get
            {
                return _gridCellSelection;
            }
            set
            {
                if (_gridCellSelection != value)   //clicking second time disables selection
                    _gridCellSelection = value;
                else
                    _gridCellSelection = null;

                Debug.Log( String.Format("Selected grid cell: {0}",
                    _gridCellSelection != null ? _gridCellSelection.coordinates.ToString() : "no grid selected" )
                    ); 
            }
        }
    }
}
