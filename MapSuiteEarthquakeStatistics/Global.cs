using System.Collections.ObjectModel;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;

namespace MapSuiteEarthquakeStatistics
{
    public static class Global
    {
        public static readonly string EarthquakeHeatLayerKey = "EarthquakeHeatLayer";
        public static readonly string EarthquakePointLayerKey = "EarthquakePointLayer";
        public static readonly string SelectMarkerLayerKey = "SelectMarkerLayer";
        public static readonly string HighlightMarkerLayerKey = "HighlightMarkerLayer";

        public static readonly string WorldMapKitOverlayKey = "WorldMapKitOverlay";
        public static readonly string OpenStreetMapOverlayKey = "OpenStreetMapOverlay";
        public static readonly string BingMapsAerialOverlayKey = "BingMapsAerialOverlay";
        public static readonly string BingMapsRoadOverlayKey = "BingMapsRoadOverlay";
        public static readonly string EarthquakeOverlayKey = "EarthquakeOverlay";
        public static readonly string HighlightOverlayKey = "HighlightOverlay";

        public static readonly string PREFS_BINGMAPKEY = "BingMapKey";
        public static readonly string PREFS_NAME = "SamplePrefsFile";

        private static Collection<Feature> backupQueriedFeatures;

        static Global()
        {
            QueryConfiguration = new QueryConfiguration();
            backupQueriedFeatures = new Collection<Feature>();
        }

        public static MapView MapView { get; set; }

        public static QueryConfiguration QueryConfiguration { get; set; }

        public static BaseMapType BaseMapType { get; set; }

        public static WorldMapKitOverlay WorldMapKitOverlay
        {
            get { return (WorldMapKitOverlay)MapView.Overlays[WorldMapKitOverlayKey]; }
        }

        public static Overlay OpenStreetMapOverlay
        {
            get { return MapView.Overlays[OpenStreetMapOverlayKey]; }
        }

        public static BingMapsOverlay BingMapsAerialOverlay
        {
            get { return (BingMapsOverlay)MapView.Overlays[BingMapsAerialOverlayKey]; }
        }

        public static BingMapsOverlay BingMapsRoadOverlay
        {
            get { return (BingMapsOverlay)MapView.Overlays[BingMapsRoadOverlayKey]; }
        }

        public static void ClearBackupQueriedFeatures()
        {
            backupQueriedFeatures.Clear();
        }

        public static void BackupQueriedFeatures(Collection<Feature> queriedFeatures)
        {
            backupQueriedFeatures.Clear();
            foreach (var item in queriedFeatures)
            {
                backupQueriedFeatures.Add(item);
            }
        }

        public static Collection<Feature> GetBackupQueriedFeatures()
        {
            return new Collection<Feature>(backupQueriedFeatures);
        }

        public static Proj4Projection GetWgs84ToMercatorProjection()
        {
            Proj4Projection wgs84ToMercatorProjection = new Proj4Projection();
            wgs84ToMercatorProjection.InternalProjectionParametersString = Proj4Projection.GetWgs84ParametersString();
            wgs84ToMercatorProjection.ExternalProjectionParametersString = Proj4Projection.GetBingMapParametersString();
            return wgs84ToMercatorProjection;
        }

        public static void FilterSelectedEarthquakeFeatures(Collection<Feature> features)
        {
            LayerOverlay highlightOverlay = Global.MapView.Overlays[Global.HighlightOverlayKey] as LayerOverlay;
            InMemoryFeatureLayer selectMarkerLayer = highlightOverlay.Layers[Global.SelectMarkerLayerKey] as InMemoryFeatureLayer;

            selectMarkerLayer.InternalFeatures.Clear();

            foreach (var feature in features)
            {
                double year, depth, magnitude;
                double.TryParse(feature.ColumnValues["MAGNITUDE"], out magnitude);
                double.TryParse(feature.ColumnValues["DEPTH_KM"], out depth);
                double.TryParse(feature.ColumnValues["YEAR"], out year);

                if ((magnitude >= Global.QueryConfiguration.LowerMagnitude && magnitude <= Global.QueryConfiguration.UpperMagnitude || magnitude == -9999)
                       && (depth <= Global.QueryConfiguration.UpperDepth && depth >= Global.QueryConfiguration.LowerDepth || depth == -9999)
                       && (year >= Global.QueryConfiguration.LowerYear && year <= Global.QueryConfiguration.UpperYear) || year == -9999)
                {
                    selectMarkerLayer.InternalFeatures.Add(feature);
                }
            }

            highlightOverlay.Refresh();
        }
    }
}