import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import { getApiProducts, type ApiProduct } from '../services/apiProduct';

const ApiCreate: React.FC = () => {
  const [name, setName] = useState('');
  const [route, setRoute] = useState('');
  const [upstreamUrl, setUpstreamUrl] = useState('');
  const [methods, setMethods] = useState<string[]>([]);
  const [hosts, setHosts] = useState('');
  const [tags, setTags] = useState('');
  const [apiProductId, setApiProductId] = useState<string | undefined>(undefined);
  const [apiProducts, setApiProducts] = useState<ApiProduct[]>([]);
  const [errors, setErrors] = useState<{ [key: string]: string }>({});
  const navigate = useNavigate();

  useEffect(() => {
    const fetchApiProducts = async () => {
      try {
        const response = await getApiProducts();
        setApiProducts(response.data);
      } catch (error) {
        console.error('Failed to fetch API products', error);
      }
    };
    fetchApiProducts();
  }, []);

  const handleMethodChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { value, checked } = e.target;
    if (checked) {
      setMethods([...methods, value]);
    } else {
      setMethods(methods.filter((method) => method !== value));
    }
  };

  const validate = () => {
    const newErrors: { [key: string]: string } = {};
    if (!name) newErrors.name = 'Name is required';
    if (!route) newErrors.route = 'Route is required';
    if (!upstreamUrl) {
        newErrors.upstreamUrl = 'Upstream URL is required';
    } else {
        try {
            new URL(upstreamUrl);
        } catch (_) {
            newErrors.upstreamUrl = 'Upstream URL is not a valid URL';
        }
    }
    if (!apiProductId) newErrors.apiProductId = 'API Product is required';

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) {
      return;
    }
    try {
      const apiData = {
        name,
        route,
        upstreamUrl,
        methods,
        hosts: hosts.split(',').map(h => h.trim()).filter(h => h),
        tags: tags.split(',').map(t => t.trim()).filter(t => t),
        apiProductId: apiProductId,
      };
      await api.post('/apis', apiData);
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
          />
          {errors.name && <p className="text-red-500 text-xs mt-1">{errors.name}</p>}
        </div>
        <div className="mb-4">
          <label className="block text-gray-700 dark:text-gray-300">API Product</label>
          <select
            value={apiProductId || ''}
            onChange={(e) => setApiProductId(e.target.value || undefined)}
            className="w-full px-3 py-2 border rounded dark:bg-gray-700 dark:border-gray-600"
          >
            <option value="">Select a Product</option>
            {apiProducts.map(product => (
              <option key={product.id} value={product.id}>
                {product.name}
              </option>
            ))}
          </select>
          {errors.apiProductId && <p className="text-red-500 text-xs mt-1">{errors.apiProductId}</p>}
        </div>
        <div className="mb-4">
          <label className="block text-gray-700 dark:text-gray-300">Route</label>
          <input
            type="text"
            value={route}
            onChange={(e) => setRoute(e.target.value)}
            className="w-full px-3 py-2 border rounded dark:bg-gray-700 dark:border-gray-600"
          />
          {errors.route && <p className="text-red-500 text-xs mt-1">{errors.route}</p>}
        </div>
        <div className="mb-4">
          <label className="block text-gray-700 dark:text-gray-300">Upstream URL</label>
          <input
            type="text"
            value={upstreamUrl}
            onChange={(e) => setUpstreamUrl(e.target.value)}
            className="w-full px-3 py-2 border rounded dark:bg-gray-700 dark:border-gray-600"
          />
          {errors.upstreamUrl && <p className="text-red-500 text-xs mt-1">{errors.upstreamUrl}</p>}
        </div>
        <div className="mb-4">
          <label className="block text-gray-700 dark:text-gray-300">Hosts (comma-separated)</label>
          <input
            type="text"
            value={hosts}
            onChange={(e) => setHosts(e.target.value)}
            className="w-full px-3 py-2 border rounded dark:bg-gray-700 dark:border-gray-600"
          />
        </div>
        <div className="mb-4">
          <label className="block text-gray-700 dark:text-gray-300">Tags (comma-separated)</label>
          <input
            type="text"
            value={tags}
            onChange={(e) => setTags(e.target.value)}
            className="w-full px-3 py-2 border rounded dark:bg-gray-700 dark:border-gray-600"
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
