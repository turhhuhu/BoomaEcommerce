import React, { Component } from "react";
import { connect } from "react-redux";
import StoreProduct from "./storeProduct";
import ProductViewHeader from "./productViewHeader";

class StoreProductView extends Component {
  state = {};

  componentDidMount() {
    console.log("mounted");
  }
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
              <StoreProduct
                key={product.guid}
                storeGuid={product.storeGuid}
                guid={product.guid}
                name={product.name}
                rating={product.rating}
                price={product.price}
                amount={product.amount}
                category={product.category}
                storeName={this.props.storeInfo.storeName}
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
    storeInfo: store.store.storeInfo,
    products: store.store.products,
    filteredProducts: store.store.filteredProducts,
  };
};

export default connect(mapStateToProps)(StoreProductView);
