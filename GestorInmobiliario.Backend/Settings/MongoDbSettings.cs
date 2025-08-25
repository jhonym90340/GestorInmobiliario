namespace GestorInmobiliario.Backend.Settings
{
    // Clase para almacenar la configuración de conexión a MongoDB desde appsettings.json
    public class MongoDBSettings
    {
        // URI de conexión a MongoDB (ej: "mongodb://localhost:27017")
        public string ConnectionURI { get; set; } = string.Empty;

        // Nombre de la base de datos a usar
        public string DatabaseName { get; set; } = string.Empty;

        // Nombre de la colección para las propiedades
        public string CollectionNameProperty { get; set; } = string.Empty;

        // Nombre de la colección para los propietarios
        public string CollectionNameOwner { get; set; } = string.Empty;

        // Nombre de la colección para las imágenes de propiedades (si se gestionan por separado)
        public string CollectionNamePropertyImage { get; set; } = string.Empty;

        // Nombre de la colección para el historial de transacciones de propiedades (según PDF)
        public string CollectionNamePropertyTrace { get; set; } = string.Empty;
    }
}
