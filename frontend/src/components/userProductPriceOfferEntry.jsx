import React, { Component } from "react";
import { connect } from "react-redux";
import {
  addProductToBasket,
  createBasketWithProduct,
} from "../actions/userActions";

class UserProductPriceOfferEntry extends Component {
  state = {};
  handleAddToCart = () => {
    const product = this.props.productOffer.product;
    const price = this.props.productOffer.counterOfferPrice
      ? this.props.productOffer.counterOfferPrice
      : this.props.productOffer.offerPrice;
    let basketToAddTo = this.props.baskets.find(
      (basket) => basket.storeGuid === this.props.storeGuid
    );
    if (basketToAddTo) {
      this.props.dispatch(
        addProductToBasket(
          {
            basketGuid: basketToAddTo.guid,
            purchaseProduct: {
              productGuid: product.guid,
              amount: product.amount,
              price: price,
            },
            product: {
              name: product.name,
              category: product.category,
            },
          },
          { name: product.name, category: product.category }
        )
      );
    } else {
      this.props.dispatch(
        createBasketWithProduct(
          {
            storeGuid: product.storeGuid,
            purchaseProducts: [
              {
                productGuid: product.guid,
                amount: product.amount,
                price: price,
              },
            ],
            store: {
              storeName: product.storeMetaData.storeName,
            },
          },
          { name: product.name, category: product.category }
        )
      );
    }
  };

  handleRemoveOffer = () => {
    console.log(this.props.productOffer);
  };
  render() {
    return (
      <tr>
        <td>
          <span className="title mb-0">
            <strong>Offered price:</strong> $
            {this.props.productOffer.offerPrice}
          </span>
          <br />
          <span className="title mb-0">
            <strong>Product name:</strong>{" "}
            {this.props.productOffer.product.name}
          </span>
        </td>
        <td>
          {this.props.productOffer?.counterOfferPrice ? (
            <span className="title mb-0">
              <strong>Counter Offer price:</strong> $
              {this.props.productOffer.counterOfferPrice}
            </span>
          ) : null}
        </td>
        <td>
          <span className="title mb-0">
            <strong>State:</strong> {this.props.productOffer.state}
          </span>
        </td>
        <td className="float-right">
          <div>
            {this.props.productOffer?.counterOfferPrice ||
            this.props.productOffer?.state === "approved" ? (
              <button
                onClick={this.handleAddToCart}
                className="btn btn-outline-primary"
              >
                {" "}
                Add to cart
                <i className="ml-2 fa fa-shopping-cart"></i>
              </button>
            ) : null}
          </div>
        </td>
      </tr>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    baskets: store.user.cart.baskets,
  };
};

export default connect(mapStateToProps)(UserProductPriceOfferEntry);
