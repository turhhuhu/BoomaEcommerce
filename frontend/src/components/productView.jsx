import React, { Component } from "react";
import { connect } from "react-redux";
import Product from "./product";
import ProductViewHeader from "./productViewHeader";

class ProductView extends Component {
  state = {};

  render() {
    return (
      <React.Fragment>
        <ProductViewHeader
          amount={
            this.props.filteredProducts ? this.props.filteredProducts.length : 0
          }
        />
        <main className="col-md-12.5">
          <div className="row">
            {this.props.filteredProducts?.map((product) => (
              <Product
                key={product.guid}
                storeGuid={product.storeGuid}
                guid={product.guid}
                name={product.name}
                rating={product.rating}
                price={product.price}
                amount={product.amount}
                category={product.category}
                storeName={product.store?.storeName}
              />
            ))}
          </div>
        </main>
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

export default connect(mapStateToProps)(ProductView);
