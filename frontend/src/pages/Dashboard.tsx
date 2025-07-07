import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../services/api';

const Dashboard: React.FC = () => {
  const [apiCount, setApiCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchApiCount = async () => {
      try {
        const response = await api.get('/apis');
        setApiCount(response.data.length);
      } catch (err) {
        setError('Failed to fetch API count');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchApiCount();
  }, []);

  return (
    <div>
      <h1 className="text-2xl font-bold">Dashboard</h1>
      <p>Welcome to the CAPI Management Portal.</p>
      <div className="mt-4">
        <div className="bg-white dark:bg-gray-800 shadow-md rounded-lg p-6">
          <h2 className="text-xl font-bold">API Summary</h2>
          {loading && <p>Loading...</p>}
          {error && <p className="text-red-500">{error}</p>}
          {!loading && !error && (
            <p>Total APIs: {apiCount}</p>
          )}
          <div className="mt-4">
            <Link to="/apis" className="text-blue-500 hover:underline">View APIs</Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
