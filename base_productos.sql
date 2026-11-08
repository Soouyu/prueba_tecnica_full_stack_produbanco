
-- Tabla de Productos



CREATE TABLE IF NOT EXISTS productos (
    id          SERIAL        PRIMARY KEY,
    nombre      VARCHAR(120)  NOT NULL,
    descripcion VARCHAR(600),
    categoria   VARCHAR(100)  NOT NULL,
    precio      NUMERIC(12,2) NOT NULL DEFAULT 0 CHECK (precio >= 0),
    imagen      VARCHAR(700),
    stock       INTEGER       NOT NULL DEFAULT 0 CHECK (stock >= 0),
    activo      BOOLEAN       NOT NULL DEFAULT TRUE,
    fecha_creacion TIMESTAMP  NOT NULL DEFAULT NOW()
);

-- Tabla de Transacciones



CREATE TABLE IF NOT EXISTS transacciones (
    id             SERIAL        PRIMARY KEY,
    fecha          TIMESTAMP     NOT NULL DEFAULT NOW(),
    tipo           VARCHAR(10)   NOT NULL CHECK (tipo IN ('COMPRA', 'VENTA')),
    producto_id    INTEGER       NOT NULL,
    cantidad       INTEGER       NOT NULL CHECK (cantidad > 0),
    precio_unitario NUMERIC(12,2) NOT NULL CHECK (precio_unitario >= 0),
    precio_total   NUMERIC(12,2) NOT NULL CHECK (precio_total >= 0),
    detalle        VARCHAR(500)
);

-- datos ejemplo


INSERT INTO productos (nombre, descripcion, categoria, imagen, precio, stock) VALUES
    ('Laptop Dell Pro PC14250', 'Laptop Dell Pro 14 pulgadas FHD', 'Computadoras', 'https://i.dell.com/is/image/DellContent/content/dam/ss2/product-images/dell-client-products/notebooks/dell-pro/pc14250/media-gallery/notebook-dell-pro-pc14250nt-hd-fhd-scr-gray-gallery-5.psd?fmt=png-alpha&pscan=auto&scl=1&hei=402&wid=576&qlt=100,0&resMode=sharp2&size=576,402', 1200.00, 10),
    ('Mouse Logitech G203', 'Mouse gamer Logitech G203 LightSync RGB', 'Accesorios', 'https://www.ecuatech.com/web/image/product.template/8153/image_1920?unique=3f90e4a', 45.50, 50),
    ('Teclado Mecanico K621', 'Teclado mecanico K621 RGB wireless TKL', 'Accesorios', 'https://www.tecnosmart.com.ec/wp-content/uploads/2026/04/TECLADO-MECANICO-K621-RGB-SP-WIRELESS-TKL-BLUETOOTH-5.0-RGB-CHROMA-100-ANTI-GHOSTING-BLACK_TECLADOS_11925_1-800x800.webp', 89.90, 25),
    ('Monitor LED 19.5', 'Monitor LED 19.5 pulgadas VGA/HDMI', 'Monitores', 'https://web-pro-resources.s3.us-east-2.amazonaws.com/public/optimized-resources/product/388f9996-1754-4746-9997-820a6a5fabdd/image/monitor-led-195-vgahdmihd1366x768-600x600.jpg', 320.00, 15);

INSERT INTO transacciones (tipo, producto_id, cantidad, precio_unitario, precio_total, detalle) VALUES
    ('COMPRA', 1, 10, 1000.00, 10000.00, 'compra de laptos'),
    ('VENTA',  1, 2,  1200.00,  2400.00, 'venta a cliente'),
    ('COMPRA', 2, 50, 40.00,    2000.00, 'compra de mouses');
