import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { getApiProducts, deleteApiProduct } from '../services/apiProduct';
import type { ApiProduct } from '../services/apiProduct';

const ApiProductList: React.FC = () => {
  const [products, setProducts] = useState<ApiProduct[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const response = await getApiProducts();
        setProducts(response.data);
      } catch (err) {
        setError('Failed to fetch API products');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, []);

  const handleDelete = async (id: string) => {
    if (window.confirm('Are you sure you want to delete this product?')) {
      try {
        await deleteApiProduct(id);
        setProducts(products.filter((p) => p.id !== id));
      } catch (err) {
        setError('Failed to delete API product');
        console.error(err);
      }
    }
  };

  if (loading) return <p>Loading...</p>;
  if (error) return <p className="text-red-500">{error}</p>;

  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">API Products</h1>
        <Link to="/api-products/create" className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
          Create Product
        </Link>
      </div>
      <div className="bg-white dark:bg-gray-800 shadow-md rounded-lg">
        <table className="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead className="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Name</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Description</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">APIs</th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody className="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
            {products.map((product) => (
              <tr key={product.id}>
                <td className="px-6 py-4 whitespace-nowrap">
                  <Link to={`/api-products/${product.id}/edit`} className="text-blue-600 hover:underline">{product.name}</Link>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">{product.description}</td>
                <td className="px-6 py-4 whitespace-nowrap">{product.apiCount}</td>
                <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                  <Link to={`/api-products/${product.id}/edit`} className="text-indigo-600 hover:text-indigo-900 mr-4">Edit</Link>
                  <button onClick={() => handleDelete(product.id)} className="text-red-600 hover:text-red-900">Delete</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default ApiProductList;
