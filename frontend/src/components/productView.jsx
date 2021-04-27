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
          amount={this.props.products ? this.props.products.length : 0}
        />
        <main className="col-md-12.5">
          <div className="row">
            {this.props.products?.map((product) => (
              <Product
                key={product.guid}
                name={product.name}
                rating={product.rating}
                price={product.price}
                amount={product.amount}
                category={product.category}
              />
            ))}
          </div>
        </main>
      </React.Fragment>
    );
  }
}

export default connect()(ProductView);
