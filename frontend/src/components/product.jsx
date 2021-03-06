import React, { Component } from "react";
import { connect } from "react-redux";
import "../css/product.css";
import Rating from "./rating";
import {
  addProductToBasket,
  createBasketWithProduct,
} from "../actions/userActions";
import OfferPriceDialog from "./offerPriceDialog";

class Product extends Component {
  state = { quantity: 1, isDialogOpen: false };

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  handleInputDisable = (event) => {
    event.preventDefault();
  };

  handleOfferPrice = () => {
    this.setState({ isDialogOpen: true });
  };

  closeOfferPrice = () => {
    this.setState({ isDialogOpen: false });
  };

  handleAddToCart = () => {
    let basketToAddTo = this.props.baskets.find(
      (basket) => basket.storeGuid === this.props.storeGuid
    );
    if (basketToAddTo) {
      this.props.dispatch(
        addProductToBasket(
          {
            basketGuid: basketToAddTo.guid,
            purchaseProduct: {
              productGuid: this.props.guid,
              amount: this.state.quantity,
              price: this.props.price,
            },
            product: {
              name: this.props.name,
              category: this.props.category,
            },
          },
          {
            name: this.props.name,
            category: this.props.category,
            amount: this.props.amount,
          }
        )
      );
    } else {
      this.props.dispatch(
        createBasketWithProduct(
          {
            storeGuid: this.props.storeGuid,
            purchaseProducts: [
              {
                productGuid: this.props.guid,
                amount: this.state.quantity,
                price: this.props.price,
              },
            ],
            store: {
              storeName: this.props.storeName,
            },
          },
          {
            name: this.props.name,
            category: this.props.category,
            amount: this.props.amount,
          }
        )
      );
    }
  };
  render() {
    return (
      <div className="col-md-4">
        <figure className="card card-product-grid">
          <figcaption className="info-wrap">
            <div className="fix-height">
              <h6 className="title product-card-item">{this.props.name}</h6>
              <p className="text-muted small product-card-item">
                Store: {this.props.storeName}
                <br />
                Amount in store: {this.props.amount}
                <br />
                Category: {this.props.category}
              </p>
              <div className="product-card-item">
                <Rating rating={this.props.rating} />
              </div>
              <div className="price-wrap mt-2 product-card-item">
                <label>Price:</label>
                <span className="price"> {this.props.price}</span>
              </div>
              <div className="form-group col-md flex-grow-0">
                <label>Quantity:</label>
                <div className="input-group input-spinner">
                  <div className="input-group-prepend">
                    <button
                      onClick={
                        this.state.quantity < this.props.amount
                          ? () =>
                              this.setState({
                                quantity: this.state.quantity + 1,
                              })
                          : null
                      }
                      className="btn btn-outline-primary"
                      type="button"
                      id="button-plus"
                    >
                      {" "}
                      +{" "}
                    </button>
                  </div>
                  <input
                    type="text"
                    min="1"
                    max={this.props.amount}
                    onKeyDown={this.handleInputDisable}
                    className="form-control"
                    onChange={this.handleChange}
                    value={this.state.quantity}
                  ></input>
                  <div className="input-group-append">
                    <button
                      onClick={
                        this.state.quantity > 1
                          ? () =>
                              this.setState({
                                quantity: this.state.quantity - 1,
                              })
                          : null
                      }
                      className="btn btn-outline-primary"
                      type="button"
                      id="button-minus"
                    >
                      {" "}
                      ???{" "}
                    </button>
                  </div>
                </div>
              </div>
            </div>
            <button
              onClick={this.handleAddToCart}
              className="btn btn-outline-primary btn-block"
            >
              {" "}
              Add to cart
              <i className="ml-2 fa fa-shopping-cart"></i>
            </button>
            {this.props.isAuthenticated ? (
              <button
                onClick={this.handleOfferPrice}
                className="btn btn-outline-primary btn-block"
              >
                {" "}
                Offer new price
                <i className="ml-2 fa fa-sticky-note"></i>
              </button>
            ) : null}
            <OfferPriceDialog
              isDialogOpen={this.state.isDialogOpen}
              closeDialog={this.closeOfferPrice}
              productGuid={this.props.guid}
            />
          </figcaption>
        </figure>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    baskets: store.user.cart.baskets,
    isAuthenticated: store.auth.isAuthenticated,
  };
};

export default connect(mapStateToProps)(Product);
