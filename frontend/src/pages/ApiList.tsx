import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import api, { getApis } from '../services/api';
import type { Api } from '../services/api';

const ApiList: React.FC = () => {
  const [apis, setApis] = useState<Api[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchApis = async () => {
      try {
        const response = await getApis();
        setApis(response.data);
      } catch (err) {
        setError('Failed to fetch APIs');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchApis();
  }, []);

  const handleDelete = async (id: string) => {
    if (window.confirm('Are you sure you want to delete this API?')) {
      try {
        await api.delete(`/apis/${id}`);
        setApis(apis.filter((api) => api.id !== id));
      } catch (err) {
        setError('Failed to delete API');
        console.error(err);
      }
    }
  };

  const handleEdit = (id: string) => {
    navigate(`/apis/edit/${id}`);
  };

  const handleTest = (id: string) => {
    navigate(`/apis/test/${id}`);
  };

  if (loading) return <p>Loading...</p>;
  if (error) return <p className="text-red-500">{error}</p>;

  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">APIs</h1>
        <Link to="/apis/create" className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
          Create API
        </Link>
      </div>
      <div className="overflow-x-auto">
        <table className="min-w-full bg-white dark:bg-gray-800">
          <thead>
            <tr>
              <th className="py-2 px-4 border-b">Name</th>
              <th className="py-2 px-4 border-b">Route</th>
              <th className="py-2 px-4 border-b">Upstream URL</th>
              <th className="py-2 px-4 border-b">Methods</th>
              <th className="py-2 px-4 border-b">Status</th>
              <th className="py-2 px-4 border-b">Actions</th>
            </tr>
          </thead>
          <tbody>
            {apis.map((api) => (
              <tr key={api.id}>
                <td className="py-2 px-4 border-b">{api.name}</td>
                <td className="py-2 px-4 border-b">{api.route}</td>
                <td className="py-2 px-4 border-b">{api.upstreamUrl}</td>
                <td className="py-2 px-4 border-b">{api.methods?.join(', ')}</td>
                <td className="py-2 px-4 border-b">{api.isEnabled ? 'Enabled' : 'Disabled'}</td>
                <td className="py-2 px-4 border-b">
                  <button onClick={() => handleEdit(api.id)} className="text-blue-500 hover:underline mr-4">Edit</button>
                  <button onClick={() => handleTest(api.id)} className="text-green-500 hover:underline mr-4">Test</button>
                  <button onClick={() => handleDelete(api.id)} className="text-red-500 hover:underline">Delete</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default ApiList;
