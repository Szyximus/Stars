/*
    Copyright (c) 2018, Szymon Jakóbczyk, Paweł Płatek, Michał Mielus, Maciej Rajs, Minh Nhật Trịnh, Izabela Musztyfaga
    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

        * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
        * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation 
          and/or other materials provided with the distribution.
        * Neither the name of the [organization] nor the names of its contributors may be used to endorse or promote products derived from this software 
          without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
    HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON 
    ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
    USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using Assets.Scripts;
using Assets.Scripts.HexLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;

public static class Pathfinder
{
    static Dictionary<HexCell, KeyValuePair<int, int>> cellsVisited = new Dictionary<HexCell, KeyValuePair<int, int>>(); //cell,(weight,cost)
    static Dictionary<HexCell, KeyValuePair<int, int>> cellsToVisit = new Dictionary<HexCell, KeyValuePair<int, int>>();
    static HexCell dest;

    public static List<HexCell> CalculatePath( HexCell source, HexCell destination )
    {
        dest = destination;
        cellsVisited.Add( source, new KeyValuePair<int, int>( CalculateHeuristic( source ), 0 ) );
        AddNeighbours( source, 0 );

        while ( !cellsVisited.ContainsKey( dest ) )
            VisitNextCell();

        var path = GetPath();

        cellsVisited.Clear();
        cellsToVisit.Clear();
        dest = null;

        return path;
    }

    private static void VisitNextCell()
    {
		var sortedCells = cellsToVisit.OrderBy( x => x.Value.Key );

        var nextCell = sortedCells.First().Key;
        var nextCellWeight = sortedCells.First().Value.Key;
        var nextCellCost = sortedCells.First().Value.Value;

        cellsToVisit.Remove( nextCell );
        cellsVisited.Add( nextCell, new KeyValuePair<int, int>( nextCellWeight, nextCellCost ) );
        AddNeighbours( nextCell, nextCellCost );
    }

    private static int CalculateHeuristic( HexCell actual )
    {
		return ManhattanDistance( actual, dest );
    }

	private static int ManhattanDistance( HexCell a, HexCell b )
	{
		return Mathf.Max( 
			Mathf.Abs( a.Coordinates.X - b.Coordinates.X ), 
			Mathf.Abs( a.Coordinates.Y - b.Coordinates.Y ), 
			Mathf.Abs( a.Coordinates.Z - b.Coordinates.Z ) );
	}

    private static void AddNeighbours( HexCell source, int actualCellCost )
    {
        HexCell cell = null;
        EDirection direction = (EDirection)0;
        while( ( cell = GetNextEmptyCell( source.Coordinates, ref direction ) ) != null )
        {
            if( !cellsVisited.ContainsKey( cell ) && !cellsToVisit.ContainsKey( cell ) )
                cellsToVisit.Add( 
                    cell, 
                    new KeyValuePair<int, int>( CalculateHeuristic( cell ) + actualCellCost + 1 , actualCellCost + 1 ) );
        }

		direction = (EDirection)0;
    }

    private static HexCell GetNextEmptyCell( HexCoordinates source, ref EDirection direction )
    {
		HexCell cell;
        var grid = MonoBehaviour.FindObjectOfType<HexGrid>();

        switch ( direction )
        {
            case EDirection.TopRight:
                cell = grid.FromCoordinates( source.X, source.Z + 1 );
                break;
            case EDirection.Right:
                cell = grid.FromCoordinates( source.X + 1, source.Z );
                break;
            case EDirection.BottomRight:
                cell = grid.FromCoordinates( source.X + 1, source.Z - 1 );
                break;
            case EDirection.BottomLeft:
                cell = grid.FromCoordinates( source.X , source.Z - 1 );
                break;
            case EDirection.Left:
                cell = grid.FromCoordinates( source.X - 1, source.Z );
                break;
            case EDirection.TopLeft:
                cell = grid.FromCoordinates( source.X - 1, source.Z + 1 );
                break;
            default:
                return null;
        }

        direction++;

        if (cell != null && cell.IsEmpty() )
            return cell;
        else if ( (int)direction < 6 )
            return GetNextEmptyCell( source, ref direction );
        else
            return null;
    }

    private static List<HexCell> GetPath()
    {
        var path = new List<HexCell>();
        path.Insert( 0, dest );
        var cost = cellsVisited[dest].Value - 1;

        while( cost > 0 )
        {
            var nextNode = cellsVisited
                .Where( cell => cell.Value.Value == cost )
                .Where( cell => AreNeighbours( cell.Key, path.First() ) )
                .OrderBy( cell => cell.Value.Key )
                .First()
                .Key;

            path.Insert( 0, nextNode );
            cost--;
        }

        return path;
    }

    private static bool AreNeighbours( HexCell a, HexCell b )
    {
		if ( ManhattanDistance( a, b ) == 1 )
			return true;
        else
            return false;
    }
}



