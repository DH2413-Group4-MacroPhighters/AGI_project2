using System;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using UnityEngine;
using DelaunatorSharp.Unity.Extensions;
using UnityEngine.SocialPlatforms;

namespace DefaultNamespace
{
    public class NetworkGenerator : MonoBehaviour
    {
        public GameObject lineContainter;
        public Material electricityMaterial;
        [Range(0.0f, 3.0f)] public float lineWidth = 0.5f;
        private static GameObject[] _pointsObjects;

        public void Start()
        {
            FindPoints();
            ClearNetworkPointChange();
            CreateNetwork();
        }

        public void Update()
        {
            FindPoints();
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
            ClearNetworkRender();
            

            List<IPoint> iPoints = new List<IPoint>();
            Dictionary<IPoint, Vector3> pointMemory = new Dictionary<IPoint, Vector3>();
            Dictionary<IPoint, int> pointToIndex = new Dictionary<IPoint, int>();

            int pointIndex = 0; 
            foreach (GameObject pointObj in _pointsObjects)
            {
                
                var position = pointObj.transform.position;
                float x = position.x;
                float y = position.z;
                IPoint newPoint = new Point(x, y);

                if (!pointMemory.ContainsKey(newPoint))
                {
                    iPoints.Add(newPoint);
                    pointMemory.Add(newPoint, position);
                    pointToIndex.Add(newPoint, pointIndex);
                    pointIndex++;
                }
            }

            DrawNetwork(iPoints, pointMemory, pointToIndex, iPoints);

        }

        private void DrawNetwork(List<IPoint> points, Dictionary<IPoint, Vector3> pMemory,
            Dictionary<IPoint, int> pointToIndex, List<IPoint> iPoints)
        {
            Delaunator delNet = new Delaunator(points.ToArray());
            int nPoints = points.Count;
            List<int[]> edges = new List<int[]>();
            
            delNet.ForEachTriangleEdge(edge =>
            {

                int pIndex = pointToIndex[edge.P];
                int qIndex = pointToIndex[edge.Q];
                edges.Add(new[] {pIndex, qIndex, Mathf.RoundToInt((edge.P.ToVector2() - edge.Q.ToVector2()).magnitude)});

                //CreateLine(new[] {pMemory[edge.P], pMemory[edge.Q]}, 0.25f, 1);
            });
            
            int[][] edgesToRender = MST.GenerateMst(edges.ToArray(), nPoints, edges.Count);

            float tStamp = Time.time * 1000;
            foreach (int[] edge in edgesToRender)
            {
                IPoint P = iPoints[edge[0]];
                IPoint Q = iPoints[edge[1]];

                if (P.X < Q.X)
                {
                    (P, Q) = (Q, P);
                }
                
                CreateLine(new []{pMemory[P], pMemory[Q]}, lineWidth, tStamp, 1);
            }
        }

        private void CreateLine(Vector3[] points, float width, float timeStamp, int order = 1)
        {
            Transform container = lineContainter.transform;

            var lineGameObject = new GameObject("lineSegment: " + timeStamp);
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
            int nChild = lineContainter.transform.childCount;

            for (int i = 0; i < nChild; i++)
            {
                Transform c = lineContainter.transform.GetChild(0);
                c.parent = null;
                DestroyImmediate(c.gameObject);
            }
        }

        private void ClearNetworkPointChange()
        {
            foreach (GameObject pointObj in _pointsObjects)
            {
                pointObj.transform.hasChanged = false;
            }
        }

        private void FindPoints()
        {
            _pointsObjects = GameObject.FindGameObjectsWithTag("cPoint");
            
        }
    }
    
    public static class MST
    {
        public static int[][] GenerateMst(int[][] edges, int n, int m)
        {
            DisjointSet trees = new DisjointSet(n);
            int[][] mst = new int[n-1][];
            for (int j = 0; j < mst.GetLength(0); j++)
            {
                mst[j] = new int [2];
            }
            Array.Sort<int[]>(edges,(x,y)  =>  x[2]-y[2]);  // sort on weight
           
            int v1;
            int v2;
            int i = 0;
            bool treefound = false;
            
            foreach (int[] edge in edges)
            {
                v1 = trees.find_item(edge[0]);
                v2 = trees.find_item(edge[1]);
                if (v1 != v2){
                    trees.Union(v1, v2);
                    mst[i][0] = Math.Min(edge[0],edge[1]);
                    mst[i][1] = Math.Max(edge[0],edge[1]);
                    i++;
                }
                if(i == n-1){
                    treefound = true;
                }
            }

            if (treefound)
            {
                Array.Sort<int[]>(mst, (x, y) => x[0] == y[0] ? x[1] - y[1] : x[0] - y[0]);

                return mst;
            }
            else {
                
                return new int[][]{};
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