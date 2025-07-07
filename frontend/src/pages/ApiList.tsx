import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../services/api';

interface Api {
  id: string;
  name: string;
  upstreamUrl: string;
  route: string;
  kongId: string;
}

const ApiList: React.FC = () => {
  const [apis, setApis] = useState<Api[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchApis = async () => {
      try {
        const response = await api.get('/apis');
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
              <th className="py-2 px-4 border-b">Actions</th>
            </tr>
          </thead>
          <tbody>
            {apis.map((api) => (
              <tr key={api.id}>
                <td className="py-2 px-4 border-b">{api.name}</td>
                <td className="py-2 px-4 border-b">{api.route}</td>
                <td className="py-2 px-4 border-b">{api.upstreamUrl}</td>
                <td className="py-2 px-4 border-b">
                  <Link to={`/apis/edit/${api.id}`} className="text-blue-500 hover:underline">Edit</Link>
                  {/* Delete button to be implemented */}
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
