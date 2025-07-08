import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Layout from './components/Layout';
import Dashboard from './pages/Dashboard';
import ApiList from './pages/ApiList';
import ApiCreate from './pages/ApiCreate';
import ApiEdit from './pages/ApiEdit';
import ApiTest from './pages/ApiTest';
import ApiProductList from './pages/ApiProductList';
import ApiProductCreatePage from './pages/ApiProductCreate';
import ApiProductEditPage from './pages/ApiProductEdit';
import './App.css';

function App() {
  return (
    <Router>
      <Layout>
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/apis" element={<ApiList />} />
          <Route path="/apis/create" element={<ApiCreate />} />
          <Route path="/apis/edit/:id" element={<ApiEdit />} />
          <Route path="/apis/test/:id" element={<ApiTest />} />
          <Route path="/api-products" element={<ApiProductList />} />
          <Route path="/api-products/create" element={<ApiProductCreatePage />} />
          <Route path="/api-products/:id/edit" element={<ApiProductEditPage />} />
        </Routes>
      </Layout>
    </Router>
  );
}

export default App;
