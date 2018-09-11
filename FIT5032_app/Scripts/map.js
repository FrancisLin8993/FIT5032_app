mapboxgl.accessToken = 'pk.eyJ1IjoiZmxpbjAwMDIiLCJhIjoiY2psdWQxdnk2MGl5bTNwbWV4ajUxZmlpMCJ9.0EQIwezJXuGd3REKsOBBYg';
var map = new mapboxgl.Map({
    container: 'map',
    style: 'mapbox://styles/mapbox/streets-v10',
    zoom: 11,
    center: [145.05, -37.88]
});
//Create a popup object
var popup = new mapboxgl.Popup({ offset: 25 })
    .setText("Lin's Weight Loss Centre Address: 900 Dandenong Road, Caulfield East, VIC 3145");
//Add the marker to the map
var marker = new mapboxgl.Marker()
    .setLngLat([145.05, -37.88])
    .setPopup(popup)
    .addTo(map);
//Add the search bar to the map
map.addControl(new MapboxGeocoder({
    accessToken: mapboxgl.accessToken
}));
//Add Navigation control to the map
map.addControl(new mapboxgl.NavigationControl());