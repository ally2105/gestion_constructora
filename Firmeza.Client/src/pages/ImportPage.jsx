import { useState } from 'react';
import { Upload, FileSpreadsheet, AlertCircle, CheckCircle, X } from 'lucide-react';
import '../styles/ImportPage.css';

const ImportPage = () => {
    const [file, setFile] = useState(null);
    const [loading, setLoading] = useState(false);
    const [result, setResult] = useState(null);
    const [error, setError] = useState(null);

    const handleFileChange = (e) => {
        const selectedFile = e.target.files[0];
        if (selectedFile) {
            // Validar que sea un archivo Excel
            const validTypes = [
                'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
                'application/vnd.ms-excel'
            ];

            if (validTypes.includes(selectedFile.type) || selectedFile.name.endsWith('.xlsx') || selectedFile.name.endsWith('.xls')) {
                setFile(selectedFile);
                setError(null);
                setResult(null);
            } else {
                setError('Por favor selecciona un archivo Excel válido (.xlsx o .xls)');
                setFile(null);
            }
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!file) {
            setError('Por favor selecciona un archivo');
            return;
        }

        setLoading(true);
        setError(null);
        setResult(null);

        const formData = new FormData();
        formData.append('file', file);

        try {
            const response = await fetch('https://localhost:7240/api/import/upload', {
                method: 'POST',
                body: formData,
            });

            if (!response.ok) {
                throw new Error('Error al importar el archivo');
            }

            const data = await response.json();
            setResult(data);
            setFile(null);
            // Limpiar el input file
            document.getElementById('file-input').value = '';
        } catch (err) {
            setError(err.message || 'Error al procesar el archivo');
        } finally {
            setLoading(false);
        }
    };

    const handleDragOver = (e) => {
        e.preventDefault();
        e.stopPropagation();
    };

    const handleDrop = (e) => {
        e.preventDefault();
        e.stopPropagation();

        const droppedFile = e.dataTransfer.files[0];
        if (droppedFile) {
            const event = { target: { files: [droppedFile] } };
            handleFileChange(event);
        }
    };

    return (
        <div className="import-page">
            <div className="import-container">
                <div className="import-header">
                    <FileSpreadsheet size={48} className="header-icon" />
                    <h1>Importar Datos desde Excel</h1>
                    <p>Sube un archivo Excel para importar ventas masivamente</p>
                </div>

                <div className="import-instructions">
                    <h3>📋 Formato del archivo Excel</h3>
                    <p>El archivo debe tener las siguientes columnas en orden:</p>
                    <ol>
                        <li><strong>Email Cliente</strong> - Email del cliente (obligatorio)</li>
                        <li><strong>Nombre Cliente</strong> - Nombre completo del cliente (opcional)</li>
                        <li><strong>Producto</strong> - Nombre del producto (obligatorio)</li>
                        <li><strong>Cantidad</strong> - Cantidad de productos (número entero, obligatorio)</li>
                        <li><strong>Precio Unitario</strong> - Precio por unidad (número decimal, obligatorio)</li>
                        <li><strong>Fecha Venta</strong> - Fecha de la venta (formato: dd/mm/yyyy, obligatorio)</li>
                    </ol>
                    <p className="note">
                        <AlertCircle size={16} />
                        <span>La primera fila debe contener los encabezados y será ignorada durante la importación.</span>
                    </p>
                </div>

                <form onSubmit={handleSubmit} className="import-form">
                    <div
                        className={`file-drop-zone ${file ? 'has-file' : ''}`}
                        onDragOver={handleDragOver}
                        onDrop={handleDrop}
                    >
                        <input
                            id="file-input"
                            type="file"
                            accept=".xlsx,.xls"
                            onChange={handleFileChange}
                            disabled={loading}
                        />

                        <div className="drop-zone-content">
                            <Upload size={48} />
                            {file ? (
                                <>
                                    <p className="file-name">{file.name}</p>
                                    <p className="file-size">{(file.size / 1024).toFixed(2)} KB</p>
                                    <button
                                        type="button"
                                        className="remove-file-btn"
                                        onClick={() => {
                                            setFile(null);
                                            document.getElementById('file-input').value = '';
                                        }}
                                    >
                                        <X size={16} />
                                        Remover archivo
                                    </button>
                                </>
                            ) : (
                                <>
                                    <p>Arrastra y suelta tu archivo Excel aquí</p>
                                    <p className="or-text">o</p>
                                    <label htmlFor="file-input" className="file-label">
                                        Seleccionar archivo
                                    </label>
                                </>
                            )}
                        </div>
                    </div>

                    {error && (
                        <div className="alert alert-error">
                            <AlertCircle size={20} />
                            <span>{error}</span>
                        </div>
                    )}

                    <button
                        type="submit"
                        className="submit-btn"
                        disabled={!file || loading}
                    >
                        {loading ? (
                            <>
                                <div className="spinner"></div>
                                Procesando...
                            </>
                        ) : (
                            <>
                                <Upload size={20} />
                                Importar Datos
                            </>
                        )}
                    </button>
                </form>

                {result && (
                    <div className="import-result">
                        <div className="result-header">
                            <CheckCircle size={32} className="success-icon" />
                            <h2>Importación Completada</h2>
                        </div>

                        <div className="result-stats">
                            <div className="stat-card">
                                <span className="stat-label">Filas Procesadas</span>
                                <span className="stat-value">{result.filasProcesadas}</span>
                            </div>
                            <div className="stat-card">
                                <span className="stat-label">Ventas Creadas</span>
                                <span className="stat-value success">{result.ventasCreadas}</span>
                            </div>
                            <div className="stat-card">
                                <span className="stat-label">Clientes Nuevos</span>
                                <span className="stat-value">{result.clientesNuevos}</span>
                            </div>
                            <div className="stat-card">
                                <span className="stat-label">Productos Nuevos</span>
                                <span className="stat-value">{result.productosNuevos}</span>
                            </div>
                        </div>

                        {result.errores && result.errores.length > 0 && (
                            <div className="result-errors">
                                <h3>⚠️ Errores encontrados ({result.errores.length})</h3>
                                <ul>
                                    {result.errores.map((error, index) => (
                                        <li key={index}>{error}</li>
                                    ))}
                                </ul>
                            </div>
                        )}
                    </div>
                )}
            </div>
        </div>
    );
};

export default ImportPage;
