import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getApiById, updateApi } from '../services/api';
import { getApiProducts } from '../services/apiProduct';
import type { ApiProduct } from '../services/apiProduct';
import type { Api, ApiUpdateData } from '../services/api';

interface ApiFormData {
  name: string;
  route: string;
  upstreamUrl: string;
  methods: string[];
  hosts: string;
  tags: string;
  isEnabled: boolean;
  apiProductId: string;
}

const ApiEdit: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [formData, setFormData] = useState<ApiFormData>({
    name: '',
    route: '',
    upstreamUrl: '',
    methods: [],
    hosts: '',
    tags: '',
    isEnabled: true,
    apiProductId: '',
  });
  const [apiProducts, setApiProducts] = useState<ApiProduct[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      if (!id) return;
      try {
        setLoading(true);
        const [apiResponse, productsResponse] = await Promise.all([
          getApiById(id),
          getApiProducts(),
        ]);

        const api: Api = apiResponse.data;
        setFormData({
          name: api.name,
          route: api.route,
          upstreamUrl: api.upstreamUrl,
          methods: api.methods || [],
          hosts: api.hosts?.join(', ') || '',
          tags: api.tags?.join(', ') || '',
          isEnabled: api.isEnabled,
          apiProductId: api.apiProducts?.[0]?.id || '', // Assuming one product per API for now
        });
        setApiProducts(productsResponse.data);

      } catch (err) {
        setError('Failed to fetch data. Please try again.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [id]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleCheckboxChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, checked } = e.target;
    setFormData(prev => ({ ...prev, [name]: checked }));
  };

  const handleMethodChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { value, checked } = e.target;
    setFormData(prev => {
      const newMethods = checked
        ? [...prev.methods, value]
        : prev.methods.filter(m => m !== value);
      return { ...prev, methods: newMethods };
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!id) return;

    const dataToSend: ApiUpdateData = {
      ...formData,
      hosts: formData.hosts.split(',').map(h => h.trim()).filter(Boolean),
      tags: formData.tags.split(',').map(t => t.trim()).filter(Boolean),
      apiProductId: formData.apiProductId || null,
    };

    try {
      await updateApi(id, dataToSend);
      navigate('/apis');
    } catch (err) {
      setError('Failed to update API. Please check the console for details.');
      console.error(err);
    }
  };

  if (loading) return <p>Loading...</p>;
  if (error) return <p className="text-red-500">{error}</p>;

  return (
    <div className="container mx-auto p-4">
      <h1 className="text-3xl font-bold mb-6">Edit API</h1>
      <form onSubmit={handleSubmit} className="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-md">
        
        <div className="mb-4">
          <label htmlFor="name" className="block text-gray-700 dark:text-gray-300 font-semibold mb-2">Name</label>
          <input type="text" id="name" name="name" value={formData.name} onChange={handleInputChange} className="w-full p-2 border rounded bg-gray-50 dark:bg-gray-700 dark:border-gray-600" />
        </div>

        <div className="mb-4">
          <label htmlFor="route" className="block text-gray-700 dark:text-gray-300 font-semibold mb-2">Route</label>
          <input type="text" id="route" name="route" value={formData.route} onChange={handleInputChange} className="w-full p-2 border rounded bg-gray-50 dark:bg-gray-700 dark:border-gray-600" />
        </div>

        <div className="mb-4">
          <label htmlFor="upstreamUrl" className="block text-gray-700 dark:text-gray-300 font-semibold mb-2">Upstream URL</label>
          <input type="text" id="upstreamUrl" name="upstreamUrl" value={formData.upstreamUrl} onChange={handleInputChange} className="w-full p-2 border rounded bg-gray-50 dark:bg-gray-700 dark:border-gray-600" />
        </div>

        <div className="mb-4">
          <label className="block text-gray-700 dark:text-gray-300 font-semibold mb-2">Methods</label>
          <div className="flex flex-wrap gap-4">
            {['GET', 'POST', 'PUT', 'DELETE', 'PATCH', 'OPTIONS'].map(method => (
              <label key={method} className="flex items-center space-x-2">
                <input
                  type="checkbox"
                  value={method}
                  checked={formData.methods.includes(method)}
                  onChange={handleMethodChange}
                  className="h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-500"
                />
                <span>{method}</span>
              </label>
            ))}
          </div>
        </div>

        <div className="mb-4">
          <label htmlFor="hosts" className="block text-gray-700 dark:text-gray-300 font-semibold mb-2">Hosts (comma-separated)</label>
          <input type="text" id="hosts" name="hosts" value={formData.hosts} onChange={handleInputChange} className="w-full p-2 border rounded bg-gray-50 dark:bg-gray-700 dark:border-gray-600" />
        </div>

        <div className="mb-4">
          <label htmlFor="tags" className="block text-gray-700 dark:text-gray-300 font-semibold mb-2">Tags (comma-separated)</label>
          <input type="text" id="tags" name="tags" value={formData.tags} onChange={handleInputChange} className="w-full p-2 border rounded bg-gray-50 dark:bg-gray-700 dark:border-gray-600" />
        </div>

        <div className="mb-6">
            <label className="flex items-center">
                <input
                    type="checkbox"
                    name="isEnabled"
                    checked={formData.isEnabled}
                    onChange={handleCheckboxChange}
                    className="h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-500"
                />
                <span className="ml-2 text-gray-700 dark:text-gray-300">Is Enabled?</span>
            </label>
        </div>

        <div className="mb-4">
          <label htmlFor="apiProductId" className="block text-gray-700 dark:text-gray-300 font-semibold mb-2">API Product</label>
          <select
            id="apiProductId"
            name="apiProductId"
            value={formData.apiProductId}
            onChange={handleInputChange}
            className="w-full p-2 border rounded bg-gray-50 dark:bg-gray-700 dark:border-gray-600"
          >
            <option value="">None</option>
            {apiProducts.map(product => (
              <option key={product.id} value={product.id}>
                {product.name}
              </option>
            ))}
          </select>
        </div>

        <button type="submit" className="bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded">
          Update API
        </button>
      </form>
    </div>
  );
};

export default ApiEdit;
