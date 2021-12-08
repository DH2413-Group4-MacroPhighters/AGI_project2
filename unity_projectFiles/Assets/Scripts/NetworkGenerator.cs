using System;
using System.Collections.Generic;
using DelaunatorSharp;
using UnityEngine;
using DelaunatorSharp.Unity.Extensions;

namespace DefaultNamespace
{
    public class NetworkGenerator : MonoBehaviour
    {
        public GameObject lineContainter;
        public Material electricityMaterial;
        private static GameObject[] _pointsObjects;

        public void Start()
        {
            _pointsObjects = GameObject.FindGameObjectsWithTag("cPoint");

            ClearNetworkPointChange();
            CreateNetwork();
        }

        public void Update()
        {
            foreach (GameObject pointObj in _pointsObjects)
            {
                if (pointObj.transform.hasChanged)
                {
                    CreateNetwork();
                    ClearNetworkPointChange();
                    break;
                }
            }

        }

        public void CreateNetwork()
        {
            Debug.Log("<color=yellow>Creating Network</color>");

            List<IPoint> iPoints = new List<IPoint>();
            Dictionary<IPoint, Vector3> pointMemory = new Dictionary<IPoint, Vector3>();

            foreach (GameObject pointObj in _pointsObjects)
            {
                var position = pointObj.transform.position;
                float x = position.x;
                float y = position.z;
                IPoint newPoint = new Point(x, y);
                iPoints.Add(newPoint);
                pointMemory.Add(newPoint, position);
            }

            ClearNetworkRender();
            DrawNetwork(iPoints, pointMemory);

        }

        private void DrawNetwork(List<IPoint> points, Dictionary<IPoint, Vector3> pMemory)
        {
            Delaunator delNet = new Delaunator(points.ToArray());
            int nPoints = points.Count;
            int nEdges = 0;
            int[][] edges = new int[][]{};
            
            delNet.ForEachTriangleEdge(edge =>
            {
                
                edges[nEdges] = new[] {0, 0, Mathf.RoundToInt((edge.P.ToVector2() - edge.Q.ToVector2()).magnitude)};
                nEdges++;
                
                //CreateLine(new[] {pMemory[edge.P], pMemory[edge.Q]}, 0.25f, 1);
            });
            new MST(edges, nPoints, nEdges);
        }

        private void CreateLine(Vector3[] points, float width, int order = 1)
        {
            Transform container = lineContainter.transform;

            var lineGameObject = new GameObject("lineSegment");
            lineGameObject.transform.parent = container;
            var lineRenderer = lineGameObject.AddComponent<LineRenderer>();

            lineRenderer.SetPositions(points);
            lineRenderer.numCapVertices = 5;
            lineRenderer.material = electricityMaterial;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            lineRenderer.sortingOrder = order;
        }

        private void ClearNetworkRender()
        {
            foreach (Transform child in lineContainter.transform)
            {
                child.parent = null;
                DestroyImmediate(child.gameObject);
            }
        }

        private void ClearNetworkPointChange()
        {
            foreach (GameObject pointObj in _pointsObjects)
            {
                pointObj.transform.hasChanged = false;
            }
        }
    }
    
    public class MST
    {
        public int[][] MstFinal;
        public MST(int[][] edges, int n, int m)
        {
            DisjointSet trees = new DisjointSet(n);
            int[][] mst = new int[n][];
            
            Array.Sort<int[]>(edges,(x,y)  =>  x[2]-y[2]);  // sort on weight
           
            int v1;
            int v2;
            int i = 0;
            bool treefound = false;
            
            foreach (int[] edge in edges)
            {
                v1 = trees.find_item(edge[0]);
                v2 = trees.find_item(edge[1]);
                if (!(v1 == (v2))){
                    trees.Union(v1, v2);
                    mst[i][0] = Math.Min(edge[0],edge[1]);
                    mst[i][1] = Math.Min(edge[0],edge[1]);
                    i++;
                }
                if(i == n-1){
                    treefound = true;
                }
            }

            if (treefound)
            {
                Array.Sort<int[]>(mst, (x, y) => x[0] == y[0] ? x[1] - y[1] : x[0] - y[0]);

                foreach (var e in mst)
                {
                    Console.WriteLine(e[0] + " " + e[1]);
                }
                MstFinal = mst;
            }
            else {
                
                Console.WriteLine("Impossible");
            }
        }
    }
    
    public class DisjointSet {

        static int n_items;
        public static int [] parents;
        static int [] rank;


        public DisjointSet(int n){

            n_items = n;
            parents = new int[n];
            rank = new int[n];

            for(int i= 0; i<n; i++){
                parents[i]=i;
                rank[i] = 0;
            }
        }

        public int find_item(int x) {

            while(x != parents[x]){
                //x.parent := x.parent.parent
                parents[x]= parents[parents[x]];
                // x := x.parent
                x = parents[x];

            }
            return x;
        }

        public void Union(int x, int y){

//        // Replace nodes by roots
//        x := Find(x)
//        y := Find(y)
            x = find_item(x);
            y = find_item(y);

            if(x ==y){    //        if x = y then
                return;  // x and y are already in the same set
            }

            if(rank[x] <rank[y]){
                // If necessary, rename variables to ensure that
                // x has rank at least as large as that of y
                int temp = x;
                x= y;
                y = temp;
            }
            // Make x the new root
            // y.parent := x
            parents[y] = x;

//        If necessary, increment the rank of x
//        if x.rank = y.rank then
            if(rank[x] == rank[y]){
                rank[x] +=1;
            }

        }

        public bool all_joined(){
            int root = find_item(0);
            for(int i = 0; i< n_items; i++){
                if(root != find_item(i)){
                    return false;
                }
            }
            return true;
        }

    }
    
    
}