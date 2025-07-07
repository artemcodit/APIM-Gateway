import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

const ApiCreate: React.FC = () => {
  const [name, setName] = useState('');
  const [route, setRoute] = useState('');
  const [upstreamUrl, setUpstreamUrl] = useState('');
  const [methods, setMethods] = useState<string[]>([]);
  const navigate = useNavigate();

  const handleMethodChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { value, checked } = e.target;
    if (checked) {
      setMethods([...methods, value]);
    } else {
      setMethods(methods.filter((method) => method !== value));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await api.post('/apis', { name, route, upstreamUrl, methods });
      navigate('/apis');
    } catch (error) {
      console.error('Failed to create API', error);
    }
  };

  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">Create API</h1>
      <form onSubmit={handleSubmit}>
        <div className="mb-4">
          <label className="block text-gray-700 dark:text-gray-300">Name</label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="w-full px-3 py-2 border rounded dark:bg-gray-700 dark:border-gray-600"
            required
          />
        </div>
        <div className="mb-4">
          <label className="block text-gray-700 dark:text-gray-300">Route</label>
          <input
            type="text"
            value={route}
            onChange={(e) => setRoute(e.target.value)}
            className="w-full px-3 py-2 border rounded dark:bg-gray-700 dark:border-gray-600"
            required
          />
        </div>
        <div className="mb-4">
          <label className="block text-gray-700 dark:text-gray-300">Upstream URL</label>
          <input
            type="text"
            value={upstreamUrl}
            onChange={(e) => setUpstreamUrl(e.target.value)}
            className="w-full px-3 py-2 border rounded dark:bg-gray-700 dark:border-gray-600"
            required
          />
        </div>
        <div className="mb-4">
          <label className="block text-gray-700 dark:text-gray-300">Methods</label>
          <div className="flex space-x-4">
            {['GET', 'POST', 'PUT', 'DELETE', 'PATCH'].map((method) => (
              <label key={method} className="flex items-center">
                <input
                  type="checkbox"
                  value={method}
                  onChange={handleMethodChange}
                  className="mr-2"
                />
                {method}
              </label>
            ))}
          </div>
        </div>
        <button type="submit" className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
          Create
        </button>
      </form>
    </div>
  );
};

export default ApiCreate;
