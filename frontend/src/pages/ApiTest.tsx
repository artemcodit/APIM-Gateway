import React, { useState } from 'react';
import { useParams } from 'react-router-dom';
import api from '../services/api';

const ApiTest: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const [method, setMethod] = useState('GET');
    const [headers, setHeaders] = useState('');
    const [body, setBody] = useState('');
    const [response, setResponse] = useState<any>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!id) return;

        setLoading(true);
        setError(null);
        setResponse(null);

        try {
            const headersObject = headers ? JSON.parse(headers) : {};
            const testRequest = { method, headers: headersObject, body };
            const res = await api.post(`/apis/${id}/test`, testRequest);
            setResponse(res.data);
        } catch (err: any) {
            setError(err.response?.data?.message || 'An error occurred');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container mx-auto p-4">
            <h1 className="text-3xl font-bold mb-6">Test API</h1>
            <form onSubmit={handleSubmit} className="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-md mb-6">
                <div className="mb-4">
                    <label htmlFor="method" className="block text-gray-700 dark:text-gray-300 font-semibold mb-2">Method</label>
                    <select id="method" value={method} onChange={(e) => setMethod(e.target.value)} className="w-full p-2 border rounded bg-gray-50 dark:bg-gray-700 dark:border-gray-600">
                        <option value="GET">GET</option>
                        <option value="POST">POST</option>
                        <option value="PUT">PUT</option>
                        <option value="DELETE">DELETE</option>
                        <option value="PATCH">PATCH</option>
                    </select>
                </div>
                <div className="mb-4">
                    <label htmlFor="headers" className="block text-gray-700 dark:text-gray-300 font-semibold mb-2">Headers (JSON)</label>
                    <textarea id="headers" value={headers} onChange={(e) => setHeaders(e.target.value)} className="w-full p-2 border rounded bg-gray-50 dark:bg-gray-700 dark:border-gray-600" rows={4}></textarea>
                </div>
                <div className="mb-4">
                    <label htmlFor="body" className="block text-gray-700 dark:text-gray-300 font-semibold mb-2">Body</label>
                    <textarea id="body" value={body} onChange={(e) => setBody(e.target.value)} className="w-full p-2 border rounded bg-gray-50 dark:bg-gray-700 dark:border-gray-600" rows={6}></textarea>
                </div>
                <button type="submit" className="bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded" disabled={loading}>
                    {loading ? 'Testing...' : 'Send Request'}
                </button>
            </form>

            {error && <p className="text-red-500">Error: {error}</p>}

            {response && (
                <div className="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-md">
                    <h2 className="text-2xl font-bold mb-4">Response</h2>
                    <p><strong>Status:</strong> {response.statusCode}</p>
                    <div>
                        <strong>Headers:</strong>
                        <pre className="bg-gray-100 dark:bg-gray-700 p-4 rounded mt-2">{JSON.stringify(response.headers, null, 2)}</pre>
                    </div>
                    <div>
                        <strong>Body:</strong>
                        <pre className="bg-gray-100 dark:bg-gray-700 p-4 rounded mt-2">{response.body}</pre>
                    </div>
                </div>
            )}
        </div>
    );
};

export default ApiTest;
