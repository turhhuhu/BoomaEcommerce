import React, { Component } from "react";
import { connect } from "react-redux";
import ProductView from "../components/productView";
import ProductFilter from "../components/productFilter";
import Header from "../components/header";
import "../css/productsPage.css";
import { fetchAllProducts, filterProducts } from "../actions/productsActions";

class ProductPage extends Component {
  state = {};

  componentDidMount(prevProps) {
    if (this.props !== prevProps && this.props.history.action !== "PUSH") {
      this.props.dispatch(fetchAllProducts());
    }
  }

  render() {
    return (
      <React.Fragment>
        <Header />
        <section className="section-content padding-y">
          <div className="container">
            <div className="row">
              <ProductFilter
                categories={[
                  ...new Set(
                    this.props.filteredProducts?.map(
                      (product) => product.category
                    )
                  ),
                ]}
                products={this.props.products}
                filteredProducts={this.props.filteredProducts}
                filterFunction={filterProducts}
              />
              <main className="col-md-9">
                <ProductView />
              </main>
            </div>
          </div>
        </section>
      </React.Fragment>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    products: store.products.products,
    filteredProducts: store.products.filteredProducts,
  };
};

export default connect(mapStateToProps)(ProductPage);
